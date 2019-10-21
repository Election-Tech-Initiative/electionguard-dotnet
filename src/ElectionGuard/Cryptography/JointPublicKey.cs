using System;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.Cryptography
{
    [StructLayout(LayoutKind.Sequential)]
    public struct JointPublicKey
    {
        public long Length;
        public IntPtr Bytes;
    }
}