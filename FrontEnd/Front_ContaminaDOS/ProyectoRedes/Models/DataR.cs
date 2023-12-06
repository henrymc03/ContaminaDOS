namespace ProyectoRedes.Models
{
    public class DataR
    {
        public string status { get; set; }
        public string phase { get; set; }
        public string result { get; set; }
        public string leader { get; set; }
        public string createdAt { set; get; }
        public string updatedAt { get; set; }
        public List<string> group { get; set; }
        public List<bool> votes { get; set; }
        public string id { get; set; }
    }
}
