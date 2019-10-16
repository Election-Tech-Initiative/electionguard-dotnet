using System;
using ElectionGuard.SDK.KeyCeremony.Messages;
using ElectionGuard.SDK.KeyCeremony.Trustee;

namespace ElectionGuard.SDK.KeyCeremony
{
    public class KeyCeremonyTrustee : IDisposable
    {
        private readonly UIntPtr _trustee;

        public KeyCeremonyTrustee(uint numberOfTrustees, uint threshold, uint index)
        {
            var response = TrusteeApi.NewTrustee(numberOfTrustees, threshold, index);
            if (response.Status == TrusteeStatus.Success)
            {
                _trustee = response.Trustee;
            }
        }

        public GenerateKeyReturn GenerateKey(byte[] bashHashCode)
        {
             return TrusteeApi.GenerateKey(_trustee, bashHashCode);
        }

        public GenerateSharesReturn GenerateShares(AllKeysReceivedMessage message)
        {
            return TrusteeApi.GenerateShares(_trustee, message);
        }

        public VerifySharesReturn VerifyShares(AllSharesReceivedMessage message)
        {
            return TrusteeApi.VerifyShares(_trustee, message);
        }

        public ExportStateReturn ExportState()
        {
            return TrusteeApi.ExportState(_trustee);
        }

        public void Dispose()
        {
            TrusteeApi.FreeTrustee(_trustee);
        }
    }
}