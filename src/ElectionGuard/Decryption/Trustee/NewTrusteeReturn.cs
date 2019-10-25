using System;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.Decryption.Trustee
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NewTrusteeReturn
    {
        public TrusteeStatus Status;
        public UIntPtr Trustee;
    }
}