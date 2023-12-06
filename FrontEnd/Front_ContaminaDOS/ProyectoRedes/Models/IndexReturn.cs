namespace ProyectoRedes.Models
{
    public class IndexReturn
    {
        //---
        public string ?leader { get; set; }

        public string ?result { get; set; }

        public string? phase { get; set;}

        public string? status { get; set; }

        public List<string>? group { get; set; }
        public List<bool>? votes { get; set; }

    }
}
