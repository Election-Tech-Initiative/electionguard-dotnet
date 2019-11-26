using ElectionGuard.SDK.Models.ElectionGuardAPI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ElectionGuard.SDK.Models
{
    public class SerializedBytesWithGCHandle
    {
        public SerializedBytes SerializedBytes { get; set; }
        public GCHandle Handle { get; set; }
    }
}
