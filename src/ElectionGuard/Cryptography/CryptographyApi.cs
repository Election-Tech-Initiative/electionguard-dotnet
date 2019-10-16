using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.Cryptography
{
    public static class CryptographyApi
    {
        [DllImport("electionguard", EntryPoint = "Crypto_parameters_new")]
        internal static extern void NewCryptographyParameters();

        [DllImport("electionguard", EntryPoint = "Crypto_parameters_free")]
        internal static extern void FreeCryptographyParameters();
    }
}