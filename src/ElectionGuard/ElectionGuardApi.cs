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
        /// <param name="externalIdentifier"></param>
        /// <param name="exportPath"></param>
        /// <param name="exportFileName"></param>
        /// <returns>EncryptBallotResult containing the encrypted ballot, its external id, its tracker string,
        ///     and the file that includes the ballot</returns>
        public static EncryptBallotResult EncryptBallot(
            bool[] selections, int expectedNumberOfSelected, ElectionGuardConfig electionGuardConfig, string externalIdentifier,
            string exportPath = "./", string exportFileName = "")
        {
            var apiConfig = electionGuardConfig.GetApiConfig();
            var serializedBytesWithGcHandle = ByteSerializer.ConvertFromBase64String(electionGuardConfig.JointPublicKey);
            apiConfig.SerializedJointPublicKey = serializedBytesWithGcHandle.SerializedBytes;

            var encryptedBallotMessage = new SerializedBytes();
            var trackerPtr = new IntPtr();

            try 
            {
                var success = API.EncryptBallot(
                                selections.Select(b => (byte)(b ? 1 : 0)).ToArray(),
                                (uint)expectedNumberOfSelected,
                                apiConfig,
                                externalIdentifier,
                                out encryptedBallotMessage,
                                exportPath,
                                exportFileName,
                                out IntPtr outputFilename,
                                out trackerPtr);
                if (!success)
                {
                    throw new Exception("ElectionGuardAPI EncryptBallot failed");
                }

                var result = new EncryptBallotResult()
                {

                    ExternalIdentifier = externalIdentifier,
                    EncryptedBallotMessage = ByteSerializer.ConvertToBase64String(encryptedBallotMessage),
                    Tracker = Marshal.PtrToStringAnsi(trackerPtr),
                    OutputFileName = Marshal.PtrToStringAnsi(outputFilename),
                };

                return result;
            }
            finally
            {
                // Free unmanaged memory
                API.FreeEncryptBallot(encryptedBallotMessage, trackerPtr);
                serializedBytesWithGcHandle.Handle.Free();
            }
        }

        /// <summary>
        /// Soft deletes a file by appending the system time to the filename, if it exists
        /// </summary>
        /// <param name="exportPath">file system path to export the encrypted ballot</param>
        /// <param name="filename">filename to have the system time appended to</param>
        /// <returns>boolean indicating the call succeeded or failed</returns>
        public static bool SoftDeleteEncryptedBallotsFile(
            string exportPath = "./", string exportFileName = "")
        {
            var success = API.SoftDeleteEncryptedBallotsFile(
                            exportPath,
                            exportFileName);
            if (!success)
            {
                throw new Exception("ElectionGuardAPI SoftDeleteEncryptedBallotsFile failed");
            }

            return success;
        }

        /// <summary>
        /// Loads a ballot file from the file system and batch processes the specified number of ballots starting at the specified index.
        /// </summary>
        /// <param name="startIndex">The start index of the ballots in the file to load into memory</param>
        /// <param name="count">The count of ballots to load into memory</param>
        /// <param name="numberOfSelections">The expected number of selections in each ballot being loaded into memory</param>
        /// <param name="importFilePath">the fully qualified file path of the file containing ballots to load</param>
        /// <param name="externalIdentifiers">output of the external identifiers of the ballots loaded</param>
        /// <param name="encryptedBallotMessage">output of the encrypted ballots loaded</param>
        /// <returns>an integer status code mapped to an enum array signaling success or failure state</returns>
        public static LoadBallotsResult LoadBallotsFile(
            long startIndex, long count, long numberOfSelections, string importFilePath)
        {
            var externalIdentifiers = new IntPtr[count];
            var encryptedBallotMessages = new SerializedBytes[count];

            try 
            {
                var status = (LoadBallotsStatus)API.LoadBallotsFile(
                                (ulong)startIndex,
                                (ulong)count,
                                (ulong)numberOfSelections,
                                importFilePath,
                                externalIdentifiers,
                                encryptedBallotMessages
                                );

                if (status != LoadBallotsStatus.Success)
                {
                    throw new Exception($"ElectionGuardAPI LoadBallotsFile failed: {status}");
                }

                var result = new LoadBallotsResult()
                {
                    ExternalIdentifiers = externalIdentifiers
                        .Select(p => Marshal.PtrToStringAnsi(p))
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToList(),
                    EncryptedBallotMessages = encryptedBallotMessages
                        .Where(p => p.Length > 0)
                        .Select(p => ByteSerializer.ConvertToBase64String(p))
                        .ToList()
                };

                return result;
            }
            finally
            {
                // Free unmanaged memory
                API.FreeLoadBallotsFile(null);
            }
        }

        /// <summary>
        /// Registers all the ballots and records which ballots have been cast or spoiled
        /// and exports encrypted ballots to a file
        /// </summary>
        /// <param name="electionGuardConfig"></param>
        /// <param name="encryptedBallotMessages"></param>
        /// <param name="castBallotIds"></param>
        /// <param name="spoiledBallotIds"></param>
        /// <param name="exportPath">optional, if exclused uses current working directory</param>
        /// <param name="exportFilenamePrefix">optional, if excluded uses default value</param>
        /// <returns>The filename created containing the encrypted ballots and their state (registered/cast/spoiled)</returns>
        public static RecordBallotsResult RecordBallots(ElectionGuardConfig electionGuardConfig,                                            
                                                        ICollection<string> castBallotIds,
                                                        ICollection<string> spoiledBallotIds,
                                                        ICollection<string> externalIdentifiers,
                                                        ICollection<string> encryptedBallotMessages,
                                                        string exportPath = "./",
                                                        string exportFilenamePrefix = "")
        {
            var serializedBytesWithGcHandles = encryptedBallotMessages
                                                .Select(ByteSerializer.ConvertFromBase64String);
            var bytesWithGcHandles = serializedBytesWithGcHandles.ToList();
            var ballotsArray = bytesWithGcHandles.Select(bytesWithGcHandle => bytesWithGcHandle.SerializedBytes).ToArray();
            var castArray = castBallotIds.Select(id => id).ToArray();
            var spoiledArray = spoiledBallotIds.Select(id => id).ToArray();
            var identifiersArray = externalIdentifiers.Select(id => id).ToArray();
            var castTrackerPtrs = new IntPtr[castBallotIds.Count];
            var spoiledTrackerPtrs = new IntPtr[spoiledBallotIds.Count];

            var outputFilenamePtr = new IntPtr();

            try
            {

                var success = API.RecordBallots((uint)electionGuardConfig.NumberOfSelections,
                                                (uint)castBallotIds.Count,
                                                (uint)spoiledBallotIds.Count,
                                                (ulong)encryptedBallotMessages.Count,
                                                castArray,
                                                spoiledArray,
                                                identifiersArray,
                                                ballotsArray,
                                                exportPath,
                                                exportFilenamePrefix,
                                                out outputFilenamePtr,
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

                return result;
            }
            finally
            {
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
            }
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
                                                  string exportPath = "./",
                                                  string exportFilenamePrefix = "")
        {
            var apiConfig = electionGuardConfig.GetApiConfig();
            var serializedBytesWithGcHandles = trusteeKeys.Select(ByteSerializer.ConvertFromBase64String);
            var bytesWithGcHandles = serializedBytesWithGcHandles.ToList();
            var trusteeKeysArray = bytesWithGcHandles.Select(bytesWithGcHandle => bytesWithGcHandle.SerializedBytes).ToArray();
            var tallyResults = new uint[electionGuardConfig.NumberOfSelections];

            var outputFilenamePtr = new IntPtr();

            try
            {

                var success = API.TallyVotes(apiConfig,
                                            trusteeKeysArray,
                                            (uint)numberOfTrusteesPresent,
                                            ballotsFilename,
                                            exportPath,
                                            exportFilenamePrefix,
                                            out outputFilenamePtr,
                                            tallyResults);
                if (!success)
                {
                    throw new Exception("ElectionGuardAPI TallyVotes failed");
                }

                var result = new TallyVotesResult()
                {
                    EncryptedTallyFilename = Marshal.PtrToStringAnsi(outputFilenamePtr),
                    TallyResults = tallyResults.Cast<int>().ToList(),
                };

                return result;
            }
            finally
            {
                // Free unmanaged memory
                API.FreeTallyVotes(outputFilenamePtr);
                foreach (var bytes in bytesWithGcHandles)
                {
                    bytes.Handle.Free();
                }
            }
        }
    }
}
