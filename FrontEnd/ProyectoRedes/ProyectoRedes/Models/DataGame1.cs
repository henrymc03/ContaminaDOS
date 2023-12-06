namespace ProyectoRedes.Models
{
    public class DataGame1
    {
        public string id { get; set; }

        public string name { get; set; }
        public string status { get; set; }
        public bool password { get; set; }

        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public string currentRound { get; set; }

        public List<string> players { get; set; }

        public List<string> enemies { get; set; }
    }
}
