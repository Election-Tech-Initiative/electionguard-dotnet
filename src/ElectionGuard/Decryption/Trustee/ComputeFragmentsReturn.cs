using System.Runtime.InteropServices;
using ElectionGuard.SDK.Decryption.Messages;

namespace ElectionGuard.SDK.Decryption.Trustee
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ComputeFragmentsReturn
    {
        public TrusteeStatus Status;
        public DecryptionFragments Fragments;
    }
}