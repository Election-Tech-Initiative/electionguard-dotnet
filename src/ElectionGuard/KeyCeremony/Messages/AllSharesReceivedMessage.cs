using System;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.KeyCeremony.Messages
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AllSharesReceivedMessage
    {
        public long Length;
        public UIntPtr Bytes;
    }
}