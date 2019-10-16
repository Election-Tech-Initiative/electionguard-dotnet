namespace ElectionGuard.SDK.KeyCeremony.Trustee
{
    public enum TrusteeStatus
    {
        Success = 0,
        InsufficientMemory = 1,
        InvalidParams = 2,
        PublishedPublicKeyError = 3,
        MissingPublicKey = 4,
        BadNizkp = 5,
        InvalidKeyShare = 6,
        SerializeError = 7,
        DeserializeError = 8
    }
}