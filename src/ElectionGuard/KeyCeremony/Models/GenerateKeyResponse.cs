using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.KeyCeremony.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GenerateKeyResponse
    {
        public TrusteeStatus Status;
        public KeyGeneratedMessage Message;
    }
}