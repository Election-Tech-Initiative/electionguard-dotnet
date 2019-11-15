using ElectionGuard.SDK.Config;

namespace ElectionGuard.SDK.Cryptography
{
    public static class CryptographySettings
    {
        public const int KeySize = 4096 / 8;
        public static int MaxPrivateKeySize = MaxValues.MaxTrustees * KeySize;
        public static int MaxPublicKeySize = MaxValues.MaxTrustees * KeySize;
        public const int HashDigestSizeBytes = 32;
    }
}