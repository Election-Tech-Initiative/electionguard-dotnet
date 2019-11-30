using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.ElectionGuardAPI
{
    [StructLayout(LayoutKind.Sequential)]
    public struct APIConfig
    {
        public uint NumberOfSelections;
        public uint NumberOfTrustees;
        public uint Threshold;
        public uint SubgroupOrder;
        public string ElectionMetadata;
        public SerializedBytes SerializedJointPublicKey;
    }
}