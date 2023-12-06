namespace API_ContaminaDOS.Models.DTO
{
    public class ActionResponse
    {
        public int status { get; set; }
        public string msg { get; set; }
        public ActionData data { get; set; }
    }

    public class ActionData
    {
        public string id { get; set; }
        public string leader { get; set; }
        public string status { get; set; }
        public string phase { get; set; }
        public string? result { get; set; }
        public List<string> group { get; set; }
        public List<bool> votes { get; set; }
    }
}
