using System.Runtime.InteropServices;
using ElectionGuard.SDK.Decryption.Messages;

namespace ElectionGuard.SDK.Decryption.Trustee
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ComputeShareReturn
    {
        public TrusteeStatus Status;
        public DecryptionShare Share;
    }
}