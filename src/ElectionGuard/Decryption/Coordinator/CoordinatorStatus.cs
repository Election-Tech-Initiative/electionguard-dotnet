namespace ElectionGuard.SDK.Decryption.Coordinator
{
    public enum CoordinatorStatus
    {
        Success = 0,
        InsufficientMemory = 1,
        InvalidParams = 2,
        InvalidTrusteeIndex = 3,
        DuplicateTrusteeIndex = 4,
        MissingTrustees = 5,
        IoError = 6,
        SerializeError = 7,
        DeserializeError = 8,
        ConfusedDecryptionTrustee = 9,
    };
}