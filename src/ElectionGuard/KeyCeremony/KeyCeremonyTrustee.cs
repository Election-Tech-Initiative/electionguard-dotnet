using System;
using ElectionGuard.SDK.KeyCeremony.Models;

namespace ElectionGuard.SDK.KeyCeremony
{
    public class KeyCeremonyTrustee : TrusteeWrapper, IDisposable
    {
        private readonly Trustee _trustee;

        public KeyCeremonyTrustee(uint numberOfTrustees, uint threshold, uint index)
        {
            var response = KeyCeremony_Trustee_new(numberOfTrustees, threshold, index);
            _trustee = response.Trustee;
        }

        public void Dispose()
        {
            // This may be only called once. Make pattern for that.
            KeyCeremony_Trustee_free(_trustee);
        }

        public GenerateKeyResponse GenerateKey(byte[] baseHashCode)
        {
            return KeyCeremony_Trustee_generate_key(_trustee, baseHashCode);
        }

        public GenerateSharesResponse GenerateShares(AllKeysReceivedMessage message)
        {
            return KeyCeremony_Trustee_generate_shares(_trustee, message);
        }

        public VerifySharesResponse VerifyShares(AllSharesReceivedMessage message)
        {
            return KeyCeremony_Trustee_verify_shares(_trustee, message);
        }

        public ExportStateResponse ExportState()
        {
            return KeyCeremony_Trustee_export_state(_trustee);
        }
    }
}