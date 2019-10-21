using System;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.Voting.Coordinator
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NewCoordinatorReturn
    {
        public CoordinatorStatus Status;

        public UIntPtr Coordinator;
    }
}