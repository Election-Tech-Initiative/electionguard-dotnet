using System;
using System.Collections.Generic;
using System.Text;

namespace ElectionGuard.SDK.Models
{
    public class CreateElectionResult
    {
        public ElectionGuardConfig ElectionGuardConfig { get; set; }

        public Dictionary<int, string> TrusteeKeys { get; set; }
    }
}
