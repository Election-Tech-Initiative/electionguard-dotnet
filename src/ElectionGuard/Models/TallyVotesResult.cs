using System.Collections.Generic;

namespace ElectionGuard.SDK.Models
{
    public struct TallyVotesResult
    {
        public string EncryptedTallyFilename { get; set; }
        public ICollection<int> TallyResults { get; set; }
    }
}