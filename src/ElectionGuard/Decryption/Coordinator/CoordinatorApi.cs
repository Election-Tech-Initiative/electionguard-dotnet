using System;
using System.Runtime.InteropServices;
using ElectionGuard.SDK.Decryption.Messages;
using ElectionGuard.SDK.IO;

namespace ElectionGuard.SDK.Decryption.Coordinator
{
    internal struct CoordinatorApi
    {
        [DllImport("electionguard", EntryPoint = "Decryption_Coordinator_new")]
        internal static extern NewCoordinatorReturn NewCoordinator(uint numberOfTrustees, uint threshold);

        [DllImport("electionguard", EntryPoint = "Decryption_Coordinator_free")]
        internal static extern void FreeCoordinator(UIntPtr coordinator);

        [DllImport("electionguard", EntryPoint = "Decryption_Coordinator_receive_share")]
        internal static extern CoordinatorStatus ReceiveShare(UIntPtr coordinator, DecryptionShare share);

        [DllImport("electionguard", EntryPoint = "Decryption_Coordinator_all_shares_received")]
        internal static extern AllSharesReceivedReturn AllSharesReceived(UIntPtr coordinator);

        [DllImport("electionguard", EntryPoint = "Decryption_Coordinator_receive_fragments")]
        internal static extern CoordinatorStatus ReceiveFragments(UIntPtr coordinator, DecryptionFragments fragments);

        [DllImport("electionguard", EntryPoint = "Decryption_Coordinator_all_fragments_received")]
        internal static extern CoordinatorStatus AllFragmentsReceived(UIntPtr coordinator, File file);
    }
}