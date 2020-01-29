using System;

namespace ElectionGuard.SDK.Models
{
    public enum LoadBallotsStatus
    {
        Success,
        InitializationError,
        InsufficientMemory,
        InvalidBallotIndex,
        EndOfFile,
        TimedOut,
        IoError,
        SerializeError,
        DeserializeError
    }
}