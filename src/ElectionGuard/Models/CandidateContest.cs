namespace ElectionGuard.SDK.Models
{
    public class CandidateContest : Contest
    {
        public string DistrictId { get; set; }
        public string Candidate { get; set; }
        public string Section { get; set; }
        public string Title { get; set; }
        public int Seats { get; set; }
        public Candidate[] Candidates { get; set; }
        public bool AllowWriteIns { get; set; }
    }
}