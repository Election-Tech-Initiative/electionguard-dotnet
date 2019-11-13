using System;
using ElectionGuard.SDK.Decryption.Coordinator;
using ElectionGuard.SDK.Decryption.Messages;
using ElectionGuard.SDK.IO;
using ElectionGuard.SDK.Utility;

namespace ElectionGuard.SDK.Decryption
{
    public class DecryptionCoordinator: SafePointer, IDisposable
    {
        private readonly UIntPtr _coordinator;

        public DecryptionCoordinator(int numberOfTrustees, int threshold)
        {
            var response = CoordinatorApi.NewCoordinator(Convert.ToUInt32(numberOfTrustees), Convert.ToUInt32(threshold));
            if (response.Status == CoordinatorStatus.Success)
            {
                _coordinator = response.Coordinator;
            }
            else
            {
                throw new Exception("Failed to create decryption coordinator");
            }
        }

        public CoordinatorStatus ReceiveShare(DecryptionShare share)
        {
            return Protect(_coordinator, () => CoordinatorApi.ReceiveShare(_coordinator, share));
        }

        public AllSharesReceivedReturn AllSharesReceived()
        {
            return Protect(_coordinator, () => CoordinatorApi.AllSharesReceived(_coordinator));
        }

        public CoordinatorStatus ReceiveFragments(DecryptionFragments fragments)
        {
            return Protect(_coordinator, () => CoordinatorApi.ReceiveFragments(_coordinator, fragments));
        }

        public CoordinatorStatus AllFragmentsReceived(File file)
        {
            return Protect(_coordinator, () => CoordinatorApi.AllFragmentsReceived(_coordinator, file));
        }

        public void Dispose()
        {
            ProtectVoid(_coordinator, () => CoordinatorApi.FreeCoordinator(_coordinator));
        }
    }
}