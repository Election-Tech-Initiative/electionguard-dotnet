﻿using System;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.ElectionGuardAPI
{
    internal static class ByteSerializer
    {
        /// <summary>
        /// Converts from a Serialized Bytes struct to the base 64 string
        /// representation of its byte array
        /// </summary>
        /// <param name="serializedBytes">
        ///     serialized bytes struct which contains an IntPtr to the
        ///     unmanaged byte array and the length of the byte array
        /// </param>
        /// <returns>base 64 representation of the byte array</returns>
        internal static string ConvertToBase64String(SerializedBytes serializedBytes)
        {
            // Copy the the serialized bytes pointer to a managed byte array
            var byteArray = new byte[serializedBytes.Length];
            Marshal.Copy(serializedBytes.Bytes, byteArray, 0, (int)serializedBytes.Length);

            // Convert the byte array to a base64 string that can be stored by a client
            return Convert.ToBase64String(byteArray);
        }

        /// <summary>
        /// Converts from the base64 representation of the SerializedBytes struct back to the struct
        /// </summary>
        /// <param name="bytesString">baes 64 string representing the serialized bytes data</param>
        /// <returns>serialized bytes struct that can be marshalled back to the C API</returns>
        internal static SerializedBytesWithGCHandle ConvertFromBase64String(string bytesString)
        {
            var byteArray = Convert.FromBase64String(bytesString);
            var handle = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
            var bytes = new SerializedBytes()
            {
                Length = (ulong)byteArray.Length,
                Bytes = handle.AddrOfPinnedObject(),
            };
            return new SerializedBytesWithGCHandle()
            {
                SerializedBytes = bytes,
                Handle = handle,
            };
        }
    }
}