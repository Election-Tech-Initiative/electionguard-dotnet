using System;
using System.Runtime.InteropServices;
using ElectionGuard.SDK.Config;
using ElectionGuard.SDK.Decryption.Messages;

namespace ElectionGuard.SDK.Decryption.Coordinator
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AllSharesReceivedReturn
    {
        public CoordinatorStatus Status;
        public uint NumberOfTrustees;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxValues.MaxTrustees)]
        public byte[] RequestPresent;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxValues.MaxTrustees)]
        public DecryptionFragmentsRequest[] Requests;
    }
}