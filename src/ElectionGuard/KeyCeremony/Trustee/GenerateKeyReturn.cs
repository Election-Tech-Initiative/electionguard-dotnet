using System.Runtime.InteropServices;
using ElectionGuard.SDK.KeyCeremony.Messages;

namespace ElectionGuard.SDK.KeyCeremony.Trustee
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GenerateKeyReturn
    {
        public TrusteeStatus Status;
        public KeyGeneratedMessage Message;
    }
}