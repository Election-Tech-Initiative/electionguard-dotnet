using System;
using System.Runtime.InteropServices;
using ElectionGuard.SDK.Decryption.Messages;
using ElectionGuard.SDK.IO;
using ElectionGuard.SDK.Serialization;
using ElectionGuard.SDK.StateManagement;

namespace ElectionGuard.SDK.Decryption.Trustee
{
    internal struct TrusteeApi
    {
        [DllImport("electionguard", EntryPoint = "Decryption_Trustee_new")]
        internal static extern NewTrusteeReturn NewTrustee(uint numberOfTrustees, uint threshold, uint numberOfSelections, TrusteeState trusteeState, byte[] baseHash);

        [DllImport("electionguard", EntryPoint = "Decryption_Trustee_free")]
        internal static extern void FreeTrustee(UIntPtr trustee);

        [DllImport("electionguard", EntryPoint = "Decryption_Trustee_tally_voting_record")]
        internal static extern TrusteeStatus TallyVotingRecord(UIntPtr trustee, File file);

        [DllImport("electionguard", EntryPoint = "Decryption_Trustee_compute_share")]
        internal static extern ComputeShareReturn ComputeShare(UIntPtr trustee);

        [DllImport("electionguard", EntryPoint = "Decryption_Trustee_compute_fragments")]
        internal static extern ComputeFragmentsReturn ComputeFragments(UIntPtr trustee, DecryptionFragmentsRequest fragmentsRequest);

        internal static TrusteeState NewTrusteeState(string trusteeKey)
        {
            var trusteeState = Convert.FromBase64String(trusteeKey);
            return NewTrusteeState(trusteeState);
        }

        //internal static TrusteeState NewTrusteeState(TrusteeStateExport trusteeStateExport)
        //{
        //    var trusteeState = TrusteeStateSerializer.Serialize(trusteeStateExport);
        //    return NewTrusteeState(trusteeState);
        //}

        internal static TrusteeState NewTrusteeState(byte[] trusteeState)
        {
            var unmanagedPointer = Marshal.AllocHGlobal(trusteeState.Length);
            Marshal.Copy(trusteeState, 0, unmanagedPointer, trusteeState.Length);
            return new TrusteeState()
            {
                Length = trusteeState.Length,
                Bytes = unmanagedPointer,
            };
        }

        internal static void FreeTrusteeState(TrusteeState trusteeState)
        {
            Marshal.FreeHGlobal(trusteeState.Bytes);
        }
    }
}