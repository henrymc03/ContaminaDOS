namespace ProyectoRedes.Models
{
    public class GetRounds
    {
        public int status { get; set; }
        public string msg { get; set; }
        public List<RoundData> data { get; set; }

        public class RoundData
        {
            public string id { get; set; }
            public string leader { get; set; }
            public string status { get; set; }
            public string result { get; set; }
            public string phase { get; set; }
            public List<string> group { get; set; }
            public List<bool> votes { get; set; }
        }
    }
}
