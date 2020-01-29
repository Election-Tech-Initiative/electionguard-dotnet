using System;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.ElectionGuardAPI
{
    internal static class API
    {
        /// <summary>
        /// CreateElection entry point into the C API
        /// </summary>
        /// <param name="config">contains the data needed to create a new election</param>
        /// <param name="trusteeStates">
        ///     trusteeStates an array that can be marshalled to the C library to assign
        ///     serialized bytes data which represent the trustee states
        /// </param>
        /// <returns>the success status</returns>
        [DllImport("electionguard", EntryPoint = "API_CreateElection")]
        internal static extern bool CreateElection(ref APIConfig config, [In, Out] SerializedBytes[] trusteeStates);

        /// <summary>
        /// Free memory for bytes allocated by CreateElection call
        /// </summary>
        /// <param name="jointPublicKey">the Serialized Bytes representation of the joint public key</param>
        /// <param name="trusteeStates">the array of trusteeStates as SerializedBytes</param>
        [DllImport("electionguard", EntryPoint = "API_CreateElection_free")]
        internal static extern void FreeCreateElection(SerializedBytes jointPublicKey, SerializedBytes[] trusteeStates);

        /// <summary>
        /// Encrypts the ballot selections for the given API Config
        /// </summary>
        /// <param name="selections">A byte array of selections on the ballot</param>
        /// <param name="expectedNumberOfSelected">The expected number of true selections in the byte array</param>
        /// <param name="config">The Electionguard configuration</param>
        /// <param name="externalIdentifier">an arbitrary value meaninful to the library consumer that is associated with the encrypted ballot</param>
        /// <param name="encryptedBallotMessage">The encrypted ballot message</param>
        /// <param name="exportPath">file system path to export the encrypted ballot</param>
        /// <param name="filename">filename to write the encrypted ballot to</param>
        /// <param name="outputFilename">the actual file path and name written to</param>
        /// <param name="trackerString">the ballot tracker string</param>
        /// <returns>success or failure</returns>
        [DllImport("electionguard", EntryPoint = "API_EncryptBallot")]
        internal static extern bool EncryptBallot(byte[] selections,
                                                  uint expectedNumberOfSelected,
                                                  APIConfig config,
                                                  string externalIdentifier,
                                                  out SerializedBytes encryptedBallotMessage,
                                                  string exportPath,
                                                  string filename,
                                                  out IntPtr outputFilename,
                                                  out IntPtr trackerString);

        /// <summary>
        /// Soft deletes a file by appending the system time to the filename, if it exists
        /// </summary>
        /// <param name="exportPath">file system path to export the encrypted ballot</param>
        /// <param name="filename">filename to have the system time appended to</param>
        /// <returns>the call succeeded or failed</returns>
        [DllImport("electionguard", EntryPoint = "API_EncryptBallot_soft_delete_file")]
        internal static extern bool SoftDeleteEncryptedBallotsFile(string exportPath,
                                                                   string filename);

        /// <summary>
        /// Free memory for bytes allocated by EncryptBallot call
        /// </summary>
        /// <param name="encryptedBallotMessage">the Serialized Bytes representation of the encrypted ballot message</param>
        /// <param name="tracker">the string of the tracker</param>
        [DllImport("electionguard", EntryPoint = "API_EncryptBallot_free")]
        internal static extern void FreeEncryptBallot(SerializedBytes encryptedBallotMessage, IntPtr tracker);

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
        [DllImport("electionguard", EntryPoint = "API_LoadBallots")]
        internal static extern int LoadBallotsFile(ulong startIndex,
                                                  ulong count,
                                                  ulong numberOfSelections,
                                                  string importFilePath,
                                                  [Out] IntPtr[] externalIdentifiers,
                                                  [In, Out] SerializedBytes[] encryptedBallotMessage);

        /// <summary>
        /// Free the load ballots file
        /// </summary>
        /// <param name="importFilePath">the fully qualified file path of the file containing ballots loaded</param>
        [DllImport("electionguard", EntryPoint = "API_LoadBallots_free")]
        internal static extern void FreeLoadBallotsFile(string importFilePath);

        /// <summary>
        /// Registers all the ballots and records which ballots have been casted or spoiled.
        /// Exports to ballot results to file.
        /// </summary>
        /// <param name="numberOfSelections"></param>
        /// <param name="numberOfCastedBallots"></param>
        /// <param name="numberOfSpoiledBallots"></param>
        /// <param name="numberOfTotalBallots"></param>
        /// <param name="castedBallotIds">array of casted ballot id's that are included in externalIdentifiers</param>
        /// <param name="spoiledBallotIds">array of spoiled ballot id's that are included in externalIdentifiers</param>
        /// <param name="externalIdentifiers">array of ballot id's that are to be cast or spoiled</param>
        /// <param name="encryptedBallotMessages">the encrypted ballots that are to be cast or spoiled</param>
        /// <param name="exportPath">the path to export the cast and spoiled ballots</param>
        /// <param name="exportFilenamePrefix">the filename prefix to use for the cast and spoiled ballots file</param>
        /// <returns>call success or failure</returns>
        [DllImport("electionguard", EntryPoint = "API_RecordBallots")]
        internal static extern bool RecordBallots(uint numberOfSelections,
                                                  uint numberOfCastedBallots,
                                                  uint numberOfSpoiledBallots,
                                                  ulong numberOfTotalBallots,
                                                  [In] string[] castedBallotIds,
                                                  [In] string[] spoiledBallotIds,
                                                  [In] string[] externalIdentifiers,
                                                  [In] SerializedBytes[] encryptedBallotMessages,
                                                  string exportPath,
                                                  string exportFilenamePrefix,
                                                  out IntPtr outputFilename,
                                                  [In, Out] IntPtr[] castedTrackerPtrs,
                                                  [In, Out] IntPtr[] spoiledTrackerPtrs);

        /// <summary>
        /// Free memory for bytes allocated by the RecordBallots call
        /// </summary>
        /// <param name="outputFilename"></param>
        [DllImport("electionguard", EntryPoint = "API_RecordBallots_free")]
        internal static extern void FreeRecordBallots(IntPtr outputFilename,
                                                      uint numberOfCastedBallots,
                                                      uint numberOfSpoiledBallots,
                                                      IntPtr[] castedTrackerPtrs,
                                                      IntPtr[] spoiledTrackerPtrs);

        /// <summary>
        /// Tallys the ballots file and Decrypts the results into a different output file
        /// </summary>
        /// <param name="config"></param>
        /// <param name="trusteeStates"></param>
        /// <param name="numberOfDecryptingTrustees"></param>
        /// <param name="ballotsFilename"></param>
        /// <param name="exportPath"></param>
        /// <param name="exportFilenamePrefix"></param>
        /// <param name="outputFilename"></param>
        /// <returns></returns>
        [DllImport("electionguard", EntryPoint = "API_TallyVotes")]
        internal static extern bool TallyVotes(APIConfig config,
                                               SerializedBytes[] trusteeStates,
                                               uint numberOfDecryptingTrustees,
                                               string ballotsFilename,
                                               string exportPath,
                                               string exportFilenamePrefix,
                                               out IntPtr outputFilename,
                                               [In, Out] uint[] tallyResults);

        /// <summary>
        /// Free memory for bytes allocated by the TallyVotes call
        /// </summary>
        /// <param name="outputFilename"></param>
        [DllImport("electionguard", EntryPoint = "API_TallyVotes_free")]
        internal static extern void FreeTallyVotes(IntPtr outputFilename);
    }
}