using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.ElectionGuardAPI
{
    public class SerializedBytesWithGCHandle
    {
        public SerializedBytes SerializedBytes { get; set; }
        public GCHandle Handle { get; set; }
    }
}
