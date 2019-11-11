using System;
using System.Runtime.InteropServices;
using ElectionGuard.SDK.KeyCeremony.Messages;


namespace ElectionGuard.SDK.KeyCeremony.Coordinator
{
    internal static class CoordinatorApi
    {
        [DllImport("electionguard", EntryPoint = "KeyCeremony_Coordinator_new")]
        internal static extern NewCoordinatorReturn NewCoordinator(uint numberOfTrustees, uint threshold);

        [DllImport("electionguard", EntryPoint = "KeyCeremony_Coordinator_free")]
        internal static extern void FreeCoordinator(UIntPtr coordinator);

        [DllImport("electionguard", EntryPoint = "KeyCeremony_Coordinator_receive_key_generated")]
        internal static extern CoordinatorStatus ReceiveKeyGenerated(UIntPtr coordinator, KeyGeneratedMessage message);

        [DllImport("electionguard", EntryPoint = "KeyCeremony_Coordinator_all_keys_received")]
        internal static extern AllKeysReceivedReturn AllKeysReceived(UIntPtr coordinator);

        [DllImport("electionguard", EntryPoint = "KeyCeremony_Coordinator_receive_shares_generated")]
        internal static extern CoordinatorStatus ReceiveSharesGenerated(UIntPtr coordinator, SharesGeneratedMessage message);

        [DllImport("electionguard", EntryPoint = "KeyCeremony_Coordinator_all_shares_received")]
        internal static extern AllSharesReceivedReturn AllSharesReceived(UIntPtr coordinator);

        [DllImport("electionguard", EntryPoint = "KeyCeremony_Coordinator_receive_shares_verified")]
        internal static extern CoordinatorStatus ReceiveSharesVerified(UIntPtr coordinator, SharesVerifiedMessage message);

        [DllImport("electionguard", EntryPoint = "KeyCeremony_Coordinator_publish_joint_key")]
        internal static extern PublishJointKeyReturn PublishJointKey(UIntPtr coordinator);

        internal static byte[] GetPublishedJointKey(PublishJointKeyReturn publishJointKeyReturn)
        {
            var key = new byte[publishJointKeyReturn.Key.Length];
            Marshal.Copy(publishJointKeyReturn.Key.Bytes, key, 0, (int)publishJointKeyReturn.Key.Length);
            return key;
        }
    }
}