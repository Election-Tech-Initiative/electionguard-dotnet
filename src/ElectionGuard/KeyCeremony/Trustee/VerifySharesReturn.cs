using System.Runtime.InteropServices;
using ElectionGuard.SDK.KeyCeremony.Messages;

namespace ElectionGuard.SDK.KeyCeremony.Trustee
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VerifySharesReturn
    {
        public TrusteeStatus Status;
        public SharesVerifiedMessage Message;
    }
}