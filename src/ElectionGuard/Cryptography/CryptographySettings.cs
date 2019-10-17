using ElectionGuard.SDK.Config;

namespace ElectionGuard.SDK.Cryptography
{
    public static class CryptographySettings
    {
        public const uint KeySize = 4096 / 8;
        public static uint MaxPrivateKeySize = MaxValues.MaxTrustees * KeySize;
        public static uint MaxPublicKeySize = MaxValues.MaxTrustees * KeySize;
        public const int HashDigestSizeBytes = 32;
    }
}