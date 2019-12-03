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
        /// <summary>
        /// CreateElection entry point - performs the initial KeyCermony process to generate public keys and trustee keys 
        /// </summary>
        /// <param name="initialConfig"></param>
        /// <param name="electionManifest"></param>
        /// <returns>CreateElectionResult containing the private trustee keys and
        ///     the updated ElectionGuardConfig with the public key</returns>
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
                    throw new Exception("ElectionGuardAPI CreateElection failed");
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

        /// <summary>
        /// Encrypts the ballot selections
        /// </summary>
        /// <param name="selections"></param>
        /// <param name="expectedNumberOfSelected"></param>
        /// <param name="electionGuardConfig"></param>
        /// <param name="currentNumberOfBallots"></param>
        /// <returns>EncryptBallotResult containing the encrypted ballot, its id, its tracker string,
        ///     and the updated current number of ballots that have been encrypted</returns>
        public static EncryptBallotResult EncryptBallot(bool[] selections, int expectedNumberOfSelected, ElectionGuardConfig electionGuardConfig, int currentNumberOfBallots)
        {
            var apiConfig = electionGuardConfig.GetApiConfig();
            var serializedBytesWithGCHandle = ByteSerializer.ConvertFromBase64String(electionGuardConfig.JointPublicKey);
            apiConfig.SerializedJointPublicKey = serializedBytesWithGCHandle.SerializedBytes;

            var updatedNumberOfBallots = (ulong)currentNumberOfBallots;
            var success = API.EncryptBallot(
                            selections.Select(b => (byte)(b ? 1 : 0)).ToArray(),
                            (uint)expectedNumberOfSelected,
                            apiConfig,
                            ref updatedNumberOfBallots,
                            out ulong ballotId,
                            out SerializedBytes encryptedBallotMessage,
                            out IntPtr trackerPtr);
            if (!success)
            {
                throw new Exception("ElectionGuardAPI EncryptBallot failed");
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

        /// <summary>
        /// Registers all the ballots and records which ballots have been casted or spoiled
        /// and exports encrypted ballots to a file
        /// </summary>
        /// <param name="electionGuardConfig"></param>
        /// <param name="encryptedBallotMessages"></param>
        /// <param name="castedBallotIds"></param>
        /// <param name="spoiledBallotIds"></param>
        /// <param name="exportPath"></param>
        /// <param name="exportFilenamePrefix"></param>
        /// <returns>The filename created containing the encrypted ballots and their state (registered/cast/spoiled)</returns>
        public static RecordBallotsResult RecordBallots(ElectionGuardConfig electionGuardConfig,
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
            var castedTrackerPtrs = new IntPtr[castedBallotIds.Count];
            var spoiledTrackerPtrs = new IntPtr[spoiledBallotIds.Count];

            var success = API.RecordBallots((uint)electionGuardConfig.NumberOfSelections,
                                            (uint)castedBallotIds.Count,
                                            (uint)spoiledBallotIds.Count,
                                            (ulong)encryptedBallotMessages.Count,
                                            castedArray,
                                            spoiledArray,
                                            ballotsArray,
                                            exportPath,
                                            exportFilenamePrefix,
                                            out IntPtr outputFilenamePtr,
                                            castedTrackerPtrs,
                                            spoiledTrackerPtrs);
            if (!success)
            {
                throw new Exception("ElectionGuardAPI RecordBallots failed");
            }

            var result = new RecordBallotsResult()
            {
                EncryptedBallotsFilename = Marshal.PtrToStringAnsi(outputFilenamePtr),
                CastedBallotTrackers = castedTrackerPtrs.Select(tracker => Marshal.PtrToStringAnsi(tracker)).ToList(),
                SpoiledBallotTrackers = spoiledTrackerPtrs.Select(tracker => Marshal.PtrToStringAnsi(tracker)).ToList(),
            };

            // Free unmanaged memory
            API.FreeRecordBallots(outputFilenamePtr,
                                  (uint)castedBallotIds.Count,
                                  (uint)spoiledBallotIds.Count,
                                  castedTrackerPtrs,
                                  spoiledTrackerPtrs);
            foreach (var bytes in serializedBytesWithGCHandles)
            {
                bytes.Handle.Free();
            }

            return result;
        }

        /// <summary>
        /// Tallys the ballots file and Decrypts the results into a different output file
        /// </summary>
        /// <param name="electionGuardConfig"></param>
        /// <param name="trusteeKeys"></param>
        /// <param name="numberOfTrusteesPresent"></param>
        /// <param name="ballotsFilename"></param>
        /// <param name="exportPath"></param>
        /// <param name="exportFilenamePrefix"></param>
        /// <returns>The filename created containing the tally results</returns>
        public static string TallyVotes(ElectionGuardConfig electionGuardConfig,
                                        ICollection<string> trusteeKeys,
                                        int numberOfTrusteesPresent,
                                        string ballotsFilename,
                                        string exportPath = "",
                                        string exportFilenamePrefix = "")
        {
            var apiConfig = electionGuardConfig.GetApiConfig();
            var serializedBytesWithGCHandles = trusteeKeys.Select(message => ByteSerializer.ConvertFromBase64String(message));
            var trusteeKeysArray = serializedBytesWithGCHandles.Select(result => result.SerializedBytes).ToArray();

            var success = API.TallyVotes(apiConfig,
                                         trusteeKeysArray,
                                         (uint)numberOfTrusteesPresent,
                                         ballotsFilename,
                                         exportPath,
                                         exportFilenamePrefix,
                                         out IntPtr outputFilenamePtr);
            if (!success)
            {
                throw new Exception("ElectionGuardAPI TallyVotes failed");
            }

            string outputFilename = Marshal.PtrToStringAnsi(outputFilenamePtr);

            // Free unmanaged memory
            API.FreeTallyVotes(outputFilenamePtr);
            foreach (var result in serializedBytesWithGCHandles)
            {
                result.Handle.Free();
            }

            return outputFilename;
        }

        /// <summary>
        /// Calculates the number of selections based on the given ElectionManifest
        /// </summary>
        /// <param name="electionManifest"></param>
        /// <returns>number of selections</returns>
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
