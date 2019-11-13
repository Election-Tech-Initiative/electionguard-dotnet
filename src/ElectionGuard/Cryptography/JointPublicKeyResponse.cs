namespace ElectionGuard.SDK.Cryptography
{
    public class JointPublicKeyResponse
    {
        public int Length;
        public string Raw;
        public string Base64;
        public uint NumberOfTrustees;
        public ulong[] PublicKey;
    }
}
