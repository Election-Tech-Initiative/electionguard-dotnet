namespace ElectionGuard.SDK.Models
{
    public struct EncryptBallotResult
    {
        public string EncryptedBallotMessage { get; set; }
        public long Identifier { get; set; }
        public string Tracker { get; set; }
        public long CurrentNumberOfBallots { get; set; }
    }
}