using ElectionGuard.SDK.ElectionGuardAPI;
using ElectionGuard.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK
{
    public static class Election
    {
        public static CreateElectionResult CreateElection(ElectionGuardConfig initialConfig, ElectionManifest electionManifest)
        {
            var apiConfig = initialConfig.GetApiConfig();
            apiConfig.NumberOfSelections = (uint)CalculateSelections(electionManifest);

            // Set up trustee states array to be allocated in the api
            var trusteeStates = new SerializedBytes[Constants.MaxTrustees];
            
            try
            {
                // Call the C library API to create the election and get back the joint public key bytes
                // The trusteeStates array should be filled out with the appropriate serialized returns as well
                var success = API.CreateElection(ref apiConfig, trusteeStates);

                if (!success)
                {
                    throw new Exception("ElectionGuardAPI.CreateElection failed");
                }

                ElectionGuardConfig electionGuardConfig = new ElectionGuardConfig(apiConfig);

                var trusteeKeys = new Dictionary<int, string>();
                // Iterate through the trusteeStates returned and convert each to its base64 reprentation
                for (var i = 0; i < trusteeStates.Length; i++)
                {
                    var trusteeState = trusteeStates[i];
                    if (trusteeState.Length > 0)
                    {
                        var trusteeStateKeys = ByteSerializer.ConvertToBase64String(trusteeState);
                        trusteeKeys.Add(i, trusteeStateKeys);
                    }
                }

                return new CreateElectionResult()
                {
                    ElectionGuardConfig = electionGuardConfig,
                    TrusteeKeys = trusteeKeys
                };
            }
            finally
            {
                // Free bytes in unmanaged memory
                API.FreeCreateElection(apiConfig.SerializedJointPublicKey, trusteeStates);
            }
        }

        public static EncryptBallotResult EncryptBallot(bool[] selections, ElectionGuardConfig electionGuardConfig, int currentNumberOfBallots)
        {
            var apiConfig = electionGuardConfig.GetApiConfig();
            var serializedBytesWithGCHandle = ByteSerializer.ConvertFromBase64String(electionGuardConfig.JointPublicKey);
            apiConfig.SerializedJointPublicKey = serializedBytesWithGCHandle.SerializedBytes;

            var updatedNumberOfBallots = (ulong)currentNumberOfBallots;
            var success = API.EncryptBallot(
                            selections.Select(b => (ushort)(b ? 1 : 0)).ToArray(),
                            apiConfig,
                            ref updatedNumberOfBallots,
                            out ulong ballotId,
                            out SerializedBytes encryptedBallotMessage,
                            out IntPtr trackerPtr);
            if (!success)
            {
                throw new Exception("ElectionGuardAPI.EncryptBallot failed");
            }

            var result = new EncryptBallotResult()
            {
                CurrentNumberOfBallots = (long)updatedNumberOfBallots,
                Identifier = (long)ballotId,
                EncryptedBallotMessage = ByteSerializer.ConvertToBase64String(encryptedBallotMessage),
                Tracker = Marshal.PtrToStringAnsi(trackerPtr),
            };

            // Free unmanaged memory
            API.FreeEncryptBallot(encryptedBallotMessage, trackerPtr);
            serializedBytesWithGCHandle.Handle.Free();

            return result;
        }

        public static bool RecordBallots(ElectionGuardConfig electionGuardConfig,
                                         ICollection<string> encryptedBallotMessages,
                                         ICollection<long> castedBallotIds,
                                         ICollection<long> spoiledBallotIds,
                                         string exportPath = "",
                                         string exportFilenamePrefix = "")
        {
            var serializedBytesWithGCHandles = encryptedBallotMessages
                                                .Select(message => ByteSerializer.ConvertFromBase64String(message));
            var ballotsArray = serializedBytesWithGCHandles.Select(result => result.SerializedBytes).ToArray();
            var castedArray = castedBallotIds.Select(id => (ulong)id).ToArray();
            var spoiledArray = spoiledBallotIds.Select(id => (ulong)id).ToArray();
            var success = API.RecordBallots((uint)electionGuardConfig.NumberOfSelections,
                                                         (uint)castedBallotIds.Count,
                                                         (uint)spoiledBallotIds.Count,
                                                         (ulong)encryptedBallotMessages.Count,
                                                         castedArray,
                                                         spoiledArray,
                                                         ballotsArray,
                                                         exportPath,
                                                         exportFilenamePrefix);
            // Free handles
            foreach (var result in serializedBytesWithGCHandles)
            {
                result.Handle.Free();
            }

            return success;
        }

        public static int CalculateSelections(ElectionManifest electionManifest)
        {
            var numberOfSelections = 0;
            foreach (var contest in electionManifest.Contests)
            {
                switch (contest.Type.AsContestType())
                {
                    case ContestType.YesNo:
                        numberOfSelections += 3; // Yes + No + Null Vote
                        break;
                    case ContestType.Candidate:
                        var candidateContest = (CandidateContest)contest;
                        numberOfSelections += candidateContest.Candidates.Length;
                        numberOfSelections += candidateContest.Seats; // Null Votes
                        if (candidateContest.AllowWriteIns)
                        {
                            numberOfSelections += 1 * candidateContest.Seats;
                        }
                        break;
                }
            }
            if (numberOfSelections > Constants.MaxSelections)
            {
                throw new Exception("Max Selections Exceeded.");
            }
            if (numberOfSelections == 0)
            {
                throw new Exception("Election has no selections");
            }
            return numberOfSelections;
        }
    }
}
