namespace ProyectoRedes.Models
{
    public class ModelGroup
    {
        public List<string> otherPlayers { get; set; }
        public int decade { get; set; }

        public string gameId { get; set; }
        public List<string> groups { get; set; }
        public string roundId { get; set; }
        public string player { get; set; }
        public string password { get; set; }

        public string server { get; set; }
       
    }
}
