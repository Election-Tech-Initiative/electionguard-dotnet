using System;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.Voting.Encrypter
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UniqueIdentifier
    {
        public long Length;
        public IntPtr Bytes;
    }
}