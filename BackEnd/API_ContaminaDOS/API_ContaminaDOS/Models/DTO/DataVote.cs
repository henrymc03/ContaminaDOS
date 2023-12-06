namespace API_ContaminaDOS.Models.DTO
{

    //Hecho por Milena
    public class DataVote
    {
        public string id { get; set; } = null!;

        public string leader { get; set; } = null!;

        public string? status { get; set; }

        public string? result { get; set; }

        public string? phase { get; set; }

        public ICollection<string>? group { get; set; }

        public ICollection<bool?> vote { get; set; }

    }
}
