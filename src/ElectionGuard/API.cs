using ElectionGuard.SDK.Cryptography;
using ElectionGuard.SDK.Models;
using ElectionGuard.SDK.StateManagement;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK
{
    internal static class API
    {
        /// <summary>
        /// CreateElection entry point into the C API
        /// </summary>
        /// <param name="config">contains the data needed to create a new election</param>
        /// <param name="trusteeStates">
        ///     trusteeStates an array that can be marshalled to the C library to assign
        ///     seralized bytes data which represent the trustee states
        /// </param>
        /// <returns>the SerializedBytes representation of the joint public key</returns>
        [DllImport("electionguard", EntryPoint = "API_CreateElection")]
        internal static extern SerializedBytes CreateElection(APIConfig config, [In, Out] SerializedBytes[] trusteeStates);

        /// <summary>
        /// Free memory for bytes allocated by CreateElection call
        /// </summary>
        /// <param name="jointPublicKey">the Serialized Bytes represntation of the joint public key</param>
        /// <param name="trusteeStates">the array of trusteeStates as SerializedBytes</param>
        [DllImport("electionguard", EntryPoint = "API_CreateElection_free")]
        internal static extern void FreeCreateElection(SerializedBytes jointPublicKey, SerializedBytes[] trusteeStates);
    }
}