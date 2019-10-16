using System;
using System.Runtime.InteropServices;
using ElectionGuard.SDK.KeyCeremony.Messages;


namespace ElectionGuard.SDK.KeyCeremony.Coordinator
{
    public static class CoordinatorApi
    {
        [DllImport("electionguard", EntryPoint = "KeyCeremony_Coordinator_new")]
        internal static extern NewCoordinatorReturn NewCoordinator(uint numberOfTrustees, uint threshold);

        [DllImport("electionguard", EntryPoint = "KeyCeremony_Coordinator_free")]
        internal static extern void FreeCoordinator(UIntPtr c);

        [DllImport("electionguard", EntryPoint = "KeyCeremony_Coordinator_receive_key_generated")]
        internal static extern CoordinatorStatus ReceiveKeyGenerated(UIntPtr c, KeyGeneratedMessage message);

        [DllImport("electionguard", EntryPoint = "KeyCeremony_Coordinator_all_keys_received")]
        internal static extern AllKeysReceivedReturn AllKeysReceived(UIntPtr c);

        [DllImport("electionguard", EntryPoint = "KeyCeremony_Coordinator_receive_shares_generated")]
        internal static extern CoordinatorStatus ReceiveSharesGenerated(UIntPtr c, SharesGeneratedMessage message);

        [DllImport("electionguard", EntryPoint = "KeyCeremony_Coordinator_all_shares_received")]
        internal static extern AllSharesReceivedReturn AllSharesReceived(UIntPtr c);

        [DllImport("electionguard", EntryPoint = "KeyCeremony_Coordinator_receive_shares_verified")]
        internal static extern CoordinatorStatus ReceiveSharesVerified(UIntPtr c, SharesVerifiedMessage message);

        [DllImport("electionguard", EntryPoint = "KeyCeremony_Coordinator_publish_joint_key")]
        internal static extern PublishJointKeyReturn PublishJointKey(UIntPtr c);
    }
}