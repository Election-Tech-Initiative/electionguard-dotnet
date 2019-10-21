namespace ElectionGuard.SDK.Voting.Encrypter
{
    public enum EncrypterStatus
    {
        Success = 0,
        InsufficientMemory = 1,
        SerializeError = 2,
        DeserializeError = 3,
        IoError = 4,
        UnknownError = 5,
    }
}