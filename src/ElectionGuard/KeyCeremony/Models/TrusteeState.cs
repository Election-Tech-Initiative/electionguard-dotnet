using System;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.KeyCeremony.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TrusteeState
    {
        public long Length;
        public UIntPtr Bytes;
    }
}