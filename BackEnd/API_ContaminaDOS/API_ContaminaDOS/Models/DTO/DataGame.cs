namespace API_ContaminaDOS.Models.DTO
{
    //Hecho por Milena
    public class DataGame
    {
        public string id { get; set; } = null!;

        public string name { get; set; } = null!;

        public string owner { get; set; } = null!;

        public string? status { get; set; }

        public bool password { get; set; }

        public string currentRound { get; set; } = null!;

        public string createdAt { get; set; } = null!;

        public string updatedAt { get; set; } = null!;

        public ICollection<string>? players { get; set; }
        public ICollection<string>? enemies { get; set; }

    }
}
