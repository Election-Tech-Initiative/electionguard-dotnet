namespace ElectionGuard.SDK.StateManagement
{
    public class TrusteeStateExport
    {
        public int Length;
        public string Raw;
        public string Base64;
        public uint Index;
        public uint Threshold;
        public ulong[][] PrivateKeyCoefficients;
        public ulong[] RsaD;
        public ulong[] RsaE;
        public ulong[] RsaN;
        public ulong[] RsaP;
        public ulong[] RsaQ;
        public ulong[][] EncryptedKeyShares;
    }
}