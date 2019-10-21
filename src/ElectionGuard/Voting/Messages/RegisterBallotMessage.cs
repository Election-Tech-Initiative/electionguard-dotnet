using System;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.Voting.Messages
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RegisterBallotMessage
    {
        public long Length;
        public UIntPtr Bytes;
    }
}