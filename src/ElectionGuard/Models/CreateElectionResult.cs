using System.Collections.Generic;

namespace ElectionGuard.SDK.Models
{
    public class CreateElectionResult
    {
        public ElectionGuardConfig ElectionGuardConfig { get; set; }

        public IDictionary<int, string> TrusteeKeys { get; set; }
    }
}
