using System;
using ElectionGuard.SDK.KeyCeremony.Coordinator;
using ElectionGuard.SDK.KeyCeremony.Messages;

namespace ElectionGuard.SDK.KeyCeremony
{
    public class KeyCeremonyCoordinator : IDisposable
    {
        private readonly UIntPtr _coordinator;

        public KeyCeremonyCoordinator(uint numberOfTrustees, uint threshold)
        {
            var response = CoordinatorApi.NewCoordinator(numberOfTrustees, threshold);
            if (response.Status == CoordinatorStatus.Success)
            {
                _coordinator = response.Coordinator;
            }
        }

        public CoordinatorStatus ReceiveKey(KeyGeneratedMessage message)
        {
            return CoordinatorApi.ReceiveKeyGenerated(_coordinator, message);
        }

        public AllKeysReceivedReturn AllKeysReceived()
        {
            return CoordinatorApi.AllKeysReceived(_coordinator);
        }

        public CoordinatorStatus ReceiveShares(SharesGeneratedMessage message)
        {
            return CoordinatorApi.ReceiveSharesGenerated(_coordinator, message);
        }

        public AllSharesReceivedReturn AllSharesReceived()
        {
            return CoordinatorApi.AllSharesReceived(_coordinator);
        }

        public CoordinatorStatus ReceiveSharesVerification(SharesVerifiedMessage message)
        {
            return CoordinatorApi.ReceiveSharesVerified(_coordinator, message);
        }

        public PublishJointKeyReturn PublishJointKey()
        {
            return CoordinatorApi.PublishJointKey(_coordinator);
        }

        public void Dispose()
        {
            CoordinatorApi.FreeCoordinator(_coordinator);
        }
    }
}