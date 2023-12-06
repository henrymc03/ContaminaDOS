namespace ProyectoRedes.Models
{
    public class ProposeGroup
    {
        public string gameId { get; set; }
        public string roundId { get; set; }
        public string password { get; set; }
        public string player { get; set; }

        public List<string> group { get; set; }
    }
}
