using ElectionGuard.SDK.ElectionGuardAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ElectionGuard.SDK.Models;

namespace ElectionGuard.SDK
{
    public static class ElectionGuardApi
    {
        /// <summary>
        /// CreateElection entry point - performs the initial KeyCeremony process to generate public keys and trustee keys 
        /// </summary>
        /// <param name="initialConfig"></param>
        /// <returns>CreateElectionResult containing the private trustee keys and
        ///     the updated ElectionGuardConfig with the public key</returns>
        public static CreateElectionResult CreateElection(ElectionGuardConfig initialConfig)
        {
            var apiConfig = initialConfig.GetApiConfig();

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

                var electionGuardConfig = new ElectionGuardConfig(apiConfig);

                var trusteeKeys = new Dictionary<int, string>();
                // Iterate through the trusteeStates returned and convert each to its base64 representation
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
            var serializedBytesWithGcHandle = ByteSerializer.ConvertFromBase64String(electionGuardConfig.JointPublicKey);
            apiConfig.SerializedJointPublicKey = serializedBytesWithGcHandle.SerializedBytes;

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
            serializedBytesWithGcHandle.Handle.Free();

            return result;
        }

        /// <summary>
        /// Registers all the ballots and records which ballots have been cast or spoiled
        /// and exports encrypted ballots to a file
        /// </summary>
        /// <param name="electionGuardConfig"></param>
        /// <param name="encryptedBallotMessages"></param>
        /// <param name="castBallotIds"></param>
        /// <param name="spoiledBallotIds"></param>
        /// <param name="exportPath"></param>
        /// <param name="exportFilenamePrefix"></param>
        /// <returns>The filename created containing the encrypted ballots and their state (registered/cast/spoiled)</returns>
        public static RecordBallotsResult RecordBallots(ElectionGuardConfig electionGuardConfig,
                                                        ICollection<string> encryptedBallotMessages,
                                                        ICollection<long> castBallotIds,
                                                        ICollection<long> spoiledBallotIds,
                                                        string exportPath = "",
                                                        string exportFilenamePrefix = "")
        {
            var serializedBytesWithGcHandles = encryptedBallotMessages
                                                .Select(ByteSerializer.ConvertFromBase64String);
            var bytesWithGcHandles = serializedBytesWithGcHandles.ToList();
            var ballotsArray = bytesWithGcHandles.Select(bytesWithGcHandle => bytesWithGcHandle.SerializedBytes).ToArray();
            var castArray = castBallotIds.Select(id => (ulong)id).ToArray();
            var spoiledArray = spoiledBallotIds.Select(id => (ulong)id).ToArray();
            var castTrackerPtrs = new IntPtr[castBallotIds.Count];
            var spoiledTrackerPtrs = new IntPtr[spoiledBallotIds.Count];

            var success = API.RecordBallots((uint)electionGuardConfig.NumberOfSelections,
                                            (uint)castBallotIds.Count,
                                            (uint)spoiledBallotIds.Count,
                                            (ulong)encryptedBallotMessages.Count,
                                            castArray,
                                            spoiledArray,
                                            ballotsArray,
                                            exportPath,
                                            exportFilenamePrefix,
                                            out IntPtr outputFilenamePtr,
                                            castTrackerPtrs,
                                            spoiledTrackerPtrs);
            if (!success)
            {
                throw new Exception("ElectionGuardAPI RecordBallots failed");
            }

            var result = new RecordBallotsResult()
            {
                EncryptedBallotsFilename = Marshal.PtrToStringAnsi(outputFilenamePtr),
                CastedBallotTrackers = castTrackerPtrs.Select(tracker => Marshal.PtrToStringAnsi(tracker)).ToList(),
                SpoiledBallotTrackers = spoiledTrackerPtrs.Select(tracker => Marshal.PtrToStringAnsi(tracker)).ToList(),
            };

            // Free unmanaged memory
            API.FreeRecordBallots(outputFilenamePtr,
                                  (uint)castBallotIds.Count,
                                  (uint)spoiledBallotIds.Count,
                                  castTrackerPtrs,
                                  spoiledTrackerPtrs);
            foreach (var bytes in bytesWithGcHandles)
            {
                bytes.Handle.Free();
            }

            return result;
        }

        /// <summary>
        /// Tallies the ballots file and Decrypts the results into a different output file
        /// </summary>
        /// <param name="electionGuardConfig"></param>
        /// <param name="trusteeKeys"></param>
        /// <param name="numberOfTrusteesPresent"></param>
        /// <param name="ballotsFilename"></param>
        /// <param name="exportPath"></param>
        /// <param name="exportFilenamePrefix"></param>
        /// <returns>The filename created containing the tally results</returns>
        public static TallyVotesResult TallyVotes(ElectionGuardConfig electionGuardConfig,
                                                  ICollection<string> trusteeKeys,
                                                  int numberOfTrusteesPresent,
                                                  string ballotsFilename,
                                                  string exportPath = "",
                                                  string exportFilenamePrefix = "")
        {
            var apiConfig = electionGuardConfig.GetApiConfig();
            var serializedBytesWithGcHandles = trusteeKeys.Select(ByteSerializer.ConvertFromBase64String);
            var bytesWithGcHandles = serializedBytesWithGcHandles.ToList();
            var trusteeKeysArray = bytesWithGcHandles.Select(bytesWithGcHandle => bytesWithGcHandle.SerializedBytes).ToArray();
            var tallyResults = new uint[electionGuardConfig.NumberOfSelections];

            var success = API.TallyVotes(apiConfig,
                                         trusteeKeysArray,
                                         (uint)numberOfTrusteesPresent,
                                         ballotsFilename,
                                         exportPath,
                                         exportFilenamePrefix,
                                         out IntPtr outputFilenamePtr,
                                         tallyResults);
            if (!success)
            {
                throw new Exception("ElectionGuardAPI TallyVotes failed");
            }

            var result = new TallyVotesResult()
            {
                EncryptedTallyFilename = Marshal.PtrToStringAnsi(outputFilenamePtr),
                TallyResults = tallyResults.Cast<int>().ToArray(),
            };

            // Free unmanaged memory
            API.FreeTallyVotes(outputFilenamePtr);
            foreach (var bytes in bytesWithGcHandles)
            {
                bytes.Handle.Free();
            }

            return result;
        }
    }
}
