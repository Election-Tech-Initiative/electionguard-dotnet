using ElectionGuard.SDK.Models;
using ElectionGuard.SDK.Models.ElectionGuardAPI;
using ElectionGuard.SDK.Serialization;
using System;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK
{
    public static class Vote
    {
        public static EncryptBallotResult EncryptBallot(bool[] selections, ElectionGuardConfig electionGuardConfig, int currentNumberOfBallots)
        {
            var apiConfig = electionGuardConfig.GetApiConfig();
            var serializedBytesWithGCHandle = ByteSerializer.ConvertFromBase64String(electionGuardConfig.JointPublicKey);
            apiConfig.SerializedJointPublicKey = serializedBytesWithGCHandle.SerializedBytes;

            var updatedNumberOfBallots = (ulong)currentNumberOfBallots;
            //SerializedBytes encryptedBallotMessage = new SerializedBytes();
            var success = ElectionGuardAPI.EncryptBallot(
                            selections,
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
            ElectionGuardAPI.FreeEncryptBallot(encryptedBallotMessage, trackerPtr);
            serializedBytesWithGCHandle.Handle.Free();

            return result;
        }
    }
}
