using System;
using ElectionGuard.SDK.Cryptography;
using ElectionGuard.SDK.KeyCeremony.Messages;
using ElectionGuard.SDK.KeyCeremony.Trustee;
using ElectionGuard.SDK.Serialization;
using ElectionGuard.SDK.StateManagement;
using ElectionGuard.SDK.Utility;

namespace ElectionGuard.SDK.KeyCeremony
{
    public class KeyCeremonyTrustee : SafePointer, IDisposable
    {
        private readonly UIntPtr _trustee;
        private readonly int _numberOfTrustees;

        public KeyCeremonyTrustee(int numberOfTrustees, int threshold, int index)
        {
            _numberOfTrustees = numberOfTrustees;
            var response = TrusteeApi.NewTrustee(Convert.ToUInt32(numberOfTrustees), Convert.ToUInt32(threshold), Convert.ToUInt32(index));
            if (response.Status == TrusteeStatus.Success)
            {
                _trustee = response.Trustee;
            }
            else
            {
                throw new Exception("Failed to create key ceremony trustee");
            }
        }

        public GenerateKeyReturn GenerateKey(byte[] baseHashCode)
        {
            if (baseHashCode.Length > CryptographySettings.HashDigestSizeBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(baseHashCode));
            }
            return Protect(_trustee, () => TrusteeApi.GenerateKey(_trustee, baseHashCode));
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

        public TrusteeStateExport GetExportedState(ExportStateReturn exportStateReturn)
        {
            if (exportStateReturn.Status != TrusteeStatus.Success)
            {
                throw new Exception("Failed to export trustee state");
            }
            var state = TrusteeApi.GetExportedState(exportStateReturn);
            return TrusteeStateSerializer.Deserialize(state, _numberOfTrustees);
        }

        public void Dispose()
        {
            ProtectVoid(_trustee, () => TrusteeApi.FreeTrustee(_trustee));
        }
    }
}