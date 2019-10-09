using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.KeyCeremony.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NewTrusteeResponse
    {
        public TrusteeStatus Status;
        public Trustee Trustee;
    }
}