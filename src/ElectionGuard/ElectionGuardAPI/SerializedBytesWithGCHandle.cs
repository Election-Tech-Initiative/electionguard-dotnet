using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.ElectionGuardAPI
{
    internal class SerializedBytesWithGCHandle
    {
        internal SerializedBytes SerializedBytes { get; set; }
        internal GCHandle Handle { get; set; }
    }
}
