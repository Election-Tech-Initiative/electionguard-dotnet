using System.Collections.Generic;

namespace ElectionGuard.SDK.Models
{
    public struct LoadBallotsResult
    {
        public ICollection<string> ExternalIdentifiers { get; set; }
        public ICollection<string> EncryptedBallotMessages { get; set; }
    }
}