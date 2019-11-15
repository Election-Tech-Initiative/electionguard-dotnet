namespace ElectionGuard.SDK.Models
{
    public class ElectionManifest : ElectionProfile
    {
        public BallotTrackerConfig BallotTrackerConfig { get; set; }
        public District[] Districts { get; set; }
        public Party[] Parties { get; set; }
        public Contest[] Contests { get; set; }
        public Precinct[] Precincts { get; set; }
        public BallotStyle[] BallotStyles { get; set; }
        public string SealUrl { get; set; }
    }
}