using System.Runtime.InteropServices;
using ElectionGuard.SDK.Cryptography;

namespace ElectionGuard.SDK.KeyCeremony.Coordinator
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PublishJointKeyReturn
    {
        public CoordinatorStatus Status;
        public JointPublicKey Key;
    }
}