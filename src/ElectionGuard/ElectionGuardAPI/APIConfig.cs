using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.ElectionGuardAPI
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct APIConfig
    {
        internal uint NumberOfSelections;
        internal uint NumberOfTrustees;
        internal uint Threshold;
        internal uint SubgroupOrder;
        internal string ElectionMetadata;
        internal SerializedBytes SerializedJointPublicKey;
    }
}