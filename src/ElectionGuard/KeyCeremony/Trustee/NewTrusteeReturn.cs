using System;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.KeyCeremony.Trustee
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NewTrusteeReturn
    {
        public TrusteeStatus Status;
        public UIntPtr Trustee;
    }
}