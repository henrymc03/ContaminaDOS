namespace API_ContaminaDOS.Models.DTO
{
    public class DataCreate
    {
        public string id { get; set; }
        public string name { get; set; }
        public string owner { get; set; }
        public string status { get; set; }
        public bool password { get; set; }
        public string? currentRound { get; set; }
        public string? createdAt { get; set; }
        public string? updatedAt { get; set; }
        public ICollection<string>? players { get; set; }
        public ICollection<string>? enemies { get; set; }
    }
}
