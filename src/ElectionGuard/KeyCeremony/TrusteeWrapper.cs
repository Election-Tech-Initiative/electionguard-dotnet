using System.Runtime.InteropServices;
using ElectionGuard.SDK.KeyCeremony.Models;

// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace ElectionGuard.SDK.KeyCeremony
{
    public class TrusteeWrapper
    {
        [DllImport("electionguard")]
        protected static extern NewTrusteeResponse KeyCeremony_Trustee_new(uint num_trustees, uint threshold,
            uint index);

        [DllImport("electionguard")]
        protected static extern void KeyCeremony_Trustee_free(Trustee t);

        [DllImport("electionguard")]
        protected static extern GenerateKeyResponse KeyCeremony_Trustee_generate_key(Trustee t,
            byte[] base_hash_code);

        [DllImport("electionguard")]
        protected static extern GenerateSharesResponse KeyCeremony_Trustee_generate_shares(Trustee t,
            AllKeysReceivedMessage in_message);

        [DllImport("electionguard")]
        protected static extern VerifySharesResponse KeyCeremony_Trustee_verify_shares(Trustee t,
            AllSharesReceivedMessage in_message);

        [DllImport("electionguard")]
        protected static extern ExportStateResponse KeyCeremony_Trustee_export_state(Trustee t);
    }
}