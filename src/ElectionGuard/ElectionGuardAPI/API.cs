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
        /// <param name="selections"></param>
        /// <param name="expectedNumberOfSelected"></param>
        /// <param name="config"></param>
        /// <param name="currentNumberOfBallots"></param>
        /// <param name="ballotIdPtr"></param>
        /// <param name="encryptedBallotMessage"></param>
        /// <param name="trackerPtr"></param>
        /// <returns></returns>
        [DllImport("electionguard", EntryPoint = "API_EncryptBallot")]
        internal static extern bool EncryptBallot(byte[] selections,
                                                  uint expectedNumberOfSelected,
                                                  APIConfig config,
                                                  ref ulong currentNumberOfBallots,
                                                  out ulong ballotIdPtr,
                                                  out SerializedBytes encryptedBallotMessage,
                                                  out IntPtr trackerPtr);

        /// <summary>
        /// Free memory for bytes allocated by EncryptBallot call
        /// </summary>
        /// <param name="encryptedBallotMessage">the Serialized Bytes representation of the encrypted ballot message</param>
        /// <param name="tracker">the string of the tracker</param>
        [DllImport("electionguard", EntryPoint = "API_EncryptBallot_free")]
        internal static extern void FreeEncryptBallot(SerializedBytes encryptedBallotMessage, IntPtr tracker);

        /// <summary>
        /// Registers all the ballots and records which ballots have been casted or spoiled.
        /// Exports to ballot results to file.
        /// </summary>
        /// <param name="numberOfSelections"></param>
        /// <param name="numberOfCastedBallots"></param>
        /// <param name="numberOfSpoiledBallots"></param>
        /// <param name="numberOfTotalBallots"></param>
        /// <param name="castedBallotIds"></param>
        /// <param name="spoiledBallotIds"></param>
        /// <param name="encryptedBallotMessages"></param>
        /// <param name="exportPath"></param>
        /// <param name="exportFilenamePrefix"></param>
        /// <returns></returns>
        [DllImport("electionguard", EntryPoint = "API_RecordBallots")]
        internal static extern bool RecordBallots(uint numberOfSelections,
                                                  uint numberOfCastedBallots,
                                                  uint numberOfSpoiledBallots,
                                                  ulong numberOfTotalBallots,
                                                  ulong[] castedBallotIds,
                                                  ulong[] spoiledBallotIds,
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