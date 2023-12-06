namespace ProyectoRedes.Models
{
    public class DataGame
    {
        public string id { get; set; }

        public string name { get; set; }  
        
        public string owner { get; set; }
        public string status { get; set; }
        public bool password { get; set; }

        public string currentRound { get; set; }

        public List<string> players { get; set; }

        public List<string> enemies { get; set; }

        

        
    }
}
