using System;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.KeyCeremony.Messages
{
    [StructLayout(LayoutKind.Sequential)]
    public class SharesVerifiedMessage
    {
        public long Length;
        public UIntPtr Bytes;
    }
}