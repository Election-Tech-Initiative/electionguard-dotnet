using System.Runtime.InteropServices;
using ElectionGuard.SDK.KeyCeremony.Messages;

namespace ElectionGuard.SDK.KeyCeremony.Coordinator
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AllKeysReceivedReturn
    {
        public CoordinatorStatus Status;
        public AllKeysReceivedMessage Message;
    }
}