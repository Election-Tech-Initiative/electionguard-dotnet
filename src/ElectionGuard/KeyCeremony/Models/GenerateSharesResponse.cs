using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.KeyCeremony.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GenerateSharesResponse
    {
        public TrusteeStatus Status;
        public SharesGeneratedMessage Message;
    }
}