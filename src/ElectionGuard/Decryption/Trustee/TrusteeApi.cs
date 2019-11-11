using System;
using System.Runtime.InteropServices;
using ElectionGuard.SDK.Decryption.Messages;
using ElectionGuard.SDK.IO;
using ElectionGuard.SDK.KeyCeremony;
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

        internal static TrusteeState NewTrusteeState(TrusteeStateExport trusteeStateExport)
        {
            var unmanagedPointer = Marshal.AllocHGlobal(trusteeStateExport.Length);
            Marshal.Copy(TrusteeStateSerializer.Serialize(trusteeStateExport), 0, unmanagedPointer, trusteeStateExport.Length);
            return new TrusteeState()
            {
                Length = trusteeStateExport.Length,
                Bytes = unmanagedPointer,
            };
        }

        internal static void FreeTrusteeState(TrusteeState trusteeState)
        {
            Marshal.FreeHGlobal(trusteeState.Bytes);
        }
    }
}