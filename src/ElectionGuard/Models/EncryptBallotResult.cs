namespace ElectionGuard.SDK.Models
{
    public struct EncryptBallotResult
    {
        public string EncryptedBallotMessage { get; set; }
        public string ExternalIdentifier { get; set; }
        public string Tracker { get; set; }
#nullable enable
        public string? OutputFileName { get; set; }
#nullable disable
    }
}