using System;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.Decryption.Messages
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DecryptionShare
    {
        public long Length;
        public UIntPtr Bytes;
    }
}