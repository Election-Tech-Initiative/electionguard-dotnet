namespace ElectionGuard.SDK.Models
{
    public class ElectionProfile
    {
        public string Title { get; set; }
        public string State { get; set; }
        public County County { get; set; }
        public string Date { get; set; }
    }
}