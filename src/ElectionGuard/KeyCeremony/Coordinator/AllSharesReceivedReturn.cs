using System.Runtime.InteropServices;
using ElectionGuard.SDK.KeyCeremony.Messages;

namespace ElectionGuard.SDK.KeyCeremony.Coordinator
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AllSharesReceivedReturn
    { 
        public CoordinatorStatus Status;
        public AllSharesReceivedMessage Message;
    }
}