using System;
using System.Runtime.InteropServices;
using ElectionGuard.SDK.Cryptography;
using ElectionGuard.SDK.KeyCeremony.Messages;

namespace ElectionGuard.SDK.KeyCeremony.Trustee
{
    public static class TrusteeApi
    {
        [DllImport("electionguard", EntryPoint = "KeyCeremony_Trustee_new")]
        internal static extern NewTrusteeReturn NewTrustee(uint numberOfTrustees, uint threshold, uint index);

        [DllImport("electionguard", EntryPoint = "KeyCeremony_Trustee_free")]
        internal static extern void FreeTrustee(UIntPtr trustee);

        [DllImport("electionguard", EntryPoint = "KeyCeremony_Trustee_generate_key")]
        internal static extern GenerateKeyReturn GenerateKey(UIntPtr trustee, [MarshalAs(UnmanagedType.LPArray, SizeConst = CryptographySettings.HashDigestSizeBytes)] byte[] baseHashCode);

        [DllImport("electionguard", EntryPoint = "KeyCeremony_Trustee_generate_shares")]
        internal static extern GenerateSharesReturn GenerateShares(UIntPtr trustee, AllKeysReceivedMessage inMessage);

        [DllImport("electionguard", EntryPoint = "KeyCeremony_Trustee_verify_shares")]
        internal static extern VerifySharesReturn VerifyShares(UIntPtr trustee, AllSharesReceivedMessage inMessage);

        [DllImport("electionguard", EntryPoint = "KeyCeremony_Trustee_export_state")]
        internal static extern ExportStateReturn ExportState(UIntPtr trustee);
    }
}