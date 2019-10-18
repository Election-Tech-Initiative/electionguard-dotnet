using System;
using ElectionGuard.SDK.KeyCeremony.Messages;
using ElectionGuard.SDK.KeyCeremony.Trustee;
using ElectionGuard.SDK.Utility;

namespace ElectionGuard.SDK.KeyCeremony
{
    public class KeyCeremonyTrustee : SafePointer, IDisposable
    {
        private readonly UIntPtr _trustee;

        public KeyCeremonyTrustee(uint numberOfTrustees, uint threshold, uint index)
        {
            var response = TrusteeApi.NewTrustee(numberOfTrustees, threshold, index);
            if (response.Status == TrusteeStatus.Success)
            {
                _trustee = response.Trustee;
            }
            else
            {
                throw new Exception("Failed to create key ceremony trustee");
            }
        }

        public GenerateKeyReturn GenerateKey(byte[] bashHashCode)
        {
             return Protect(_trustee, () => TrusteeApi.GenerateKey(_trustee, bashHashCode));
        }

        public GenerateSharesReturn GenerateShares(AllKeysReceivedMessage message)
        {
            return Protect(_trustee, () => TrusteeApi.GenerateShares(_trustee, message));
        }

        public VerifySharesReturn VerifyShares(AllSharesReceivedMessage message)
        {
            return Protect(_trustee, () => TrusteeApi.VerifyShares(_trustee, message));
        }

        public ExportStateReturn ExportState()
        {
            return Protect(_trustee, () => TrusteeApi.ExportState(_trustee));
        }

        public void Dispose()
        {
            ProtectVoid(_trustee, () => TrusteeApi.FreeTrustee(_trustee));
        }
    }
}