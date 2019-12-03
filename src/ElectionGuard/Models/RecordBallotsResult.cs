using System.Collections.Generic;

namespace ElectionGuard.SDK.Models
{
    public struct RecordBallotsResult
    {
        public string EncryptedBallotsFilename { get; set; }
        public ICollection<string> CastedBallotTrackers { get; set; }
        public ICollection<string> SpoiledBallotTrackers { get; set; }
    }
}