namespace ElectionGuard.SDK.Decryption.Trustee
{
    public enum TrusteeStatus
    {
        Success = 0,
        InsufficientMemory = 1,
        InvalidParams = 2,
        IoError = 3,
        MalformedInput = 4,
        SerializeError = 5,
        DeserializeError = 6,
    }
}