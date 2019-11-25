using ElectionGuard.SDK.Config;
using ElectionGuard.SDK.Cryptography;
using ElectionGuard.SDK.StateManagement;
using System;
using System.Runtime.InteropServices;

namespace ElectionGuard.SDK.Models.ElectionGuardAPI
{
    [StructLayout(LayoutKind.Sequential)]
    public struct APIEncryptBallotResult
    {
        public SerializedBytes Message;
        public ulong Identifier;
        public string Tracker;
    }
}