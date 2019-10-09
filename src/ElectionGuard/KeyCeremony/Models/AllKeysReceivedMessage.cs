using System;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.KeyCeremony.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AllKeysReceivedMessage
    {
        public long Length;
        public UIntPtr Bytes;
    }
}