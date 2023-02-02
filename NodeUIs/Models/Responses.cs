namespace NodeUIs.Models
{
    public class Responses
    {
        public List<Credentials>? connections { get; set; }
        public int total_entries { get; set; } = 0;
        public string? message { get; set; }
        public bool status { get; set; }
    }
}
