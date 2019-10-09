using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.KeyCeremony.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ExportStateResponse
    {
        public TrusteeStatus status;
        public TrusteeState state;
    }
}