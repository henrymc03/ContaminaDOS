namespace API_ContaminaDOS.Models.DTO
{
    public class GameSearch
    {
        public string name { get; set; }
        public string owner { get; set; }
        public string status { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public bool password { get; set; }
        public List<string> players { get; set; }
        public List<string> enemies { get; set; }
        public string currentRound { get; set; }
        public string id { get; set; }
    }
}
