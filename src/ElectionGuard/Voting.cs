using ElectionGuard.SDK.ElectionGuardAPI;
using ElectionGuard.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK
{
    public static class Voting
    {
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
    }
}
