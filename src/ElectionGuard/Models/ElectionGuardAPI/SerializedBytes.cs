/**
 * Generic Serialized Bytes Struct
 * Returns from the C API have several returns that are structs
 * which contain a byte array and the array's length to represent
 * the serialized data for keys, trustees, states, etc.
 */
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ElectionGuard.SDK.Models.ElectionGuardAPI
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SerializedBytes
    {
        public ulong Length;
        public IntPtr Bytes;
    }
}
