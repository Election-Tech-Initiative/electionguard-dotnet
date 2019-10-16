using System.Runtime.InteropServices;
using ElectionGuard.SDK.KeyCeremony.Messages;

namespace ElectionGuard.SDK.KeyCeremony.Trustee
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GenerateSharesReturn
    {
        public TrusteeStatus Status;
        public SharesGeneratedMessage Message;
    }
}