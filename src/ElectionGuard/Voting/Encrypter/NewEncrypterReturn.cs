using System;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.Voting.Encrypter
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NewEncrypterReturn
    {
        public EncrypterStatus Status;
        public UIntPtr Encrypter;
    }
}