using ElectionGuard.SDK.Cryptography;
using ElectionGuard.SDK.Models.ElectionGuardAPI;
using ElectionGuard.SDK.StateManagement;
using System;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK
{
    internal static class ElectionGuardAPI
    {
        /// <summary>
        /// CreateElection entry point into the C API
        /// </summary>
        /// <param name="config">contains the data needed to create a new election</param>
        /// <param name="trusteeStates">
        ///     trusteeStates an array that can be marshalled to the C library to assign
        ///     seralized bytes data which represent the trustee states
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
        /// 
        /// </summary>
        /// <param name="selections"></param>
        /// <param name="config"></param>
        /// <param name="jointPublicKey"></param>
        /// <param name="currentNumberOfBallots"></param>
        /// <returns></returns>
        [DllImport("electionguard", EntryPoint = "API_EncryptBallot")]
        internal static extern APIEncryptBallotResult EncryptBallot(bool[] selections, APIConfig config, SerializedBytes jointPublicKey, [In, Out] IntPtr currentNumberOfBallots);

        /// <summary>
        /// Free memory for bytes allocated by EncryptBallot call
        /// </summary>
        /// <param name="encryptedBallotMessage">the Serialized Bytes representation of the encrypted ballot message</param>
        /// <param name="tracker">the string of the tracker</param>
        [DllImport("electionguard", EntryPoint = "API_EncryptBallot_free")]
        internal static extern void FreeEncryptBallot(SerializedBytes encryptedBallotMessage, string tracker);
    }
}