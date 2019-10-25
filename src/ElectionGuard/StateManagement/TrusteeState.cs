using System;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.StateManagement
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TrusteeState
    {
        public long Length;
        public IntPtr Bytes;
    }
}