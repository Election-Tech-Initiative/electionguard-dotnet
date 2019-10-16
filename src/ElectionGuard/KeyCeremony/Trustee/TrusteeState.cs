using System;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.KeyCeremony.Trustee
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TrusteeState
    {
        public long Length;
        public UIntPtr Bytes;
    }
}