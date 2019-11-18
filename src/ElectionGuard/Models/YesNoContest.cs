namespace ElectionGuard.SDK.Models
{
    public class YesNoContest : Contest
    {
        public string DistrictId { get; set; }
        public string PartyId { get; set; }
        public string Section { get; set; }
        public string ShortTitle { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}