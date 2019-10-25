using System.Runtime.InteropServices;
using ElectionGuard.SDK.StateManagement;

namespace ElectionGuard.SDK.KeyCeremony.Trustee
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ExportStateReturn
    {
        public TrusteeStatus Status;
        public TrusteeState State;
    }
}