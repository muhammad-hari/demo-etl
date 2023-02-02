namespace NodeUIs.Models
{
    public class Credentials 
    { 
        public string? connection_id { get; set; }
        public string? conn_type { get; set; }
        public string? description { get; set; } 
        public string? host { get; set; }
        public string? login { get; set; }
        public string? schema { get; set; }
        public string? password { get; set; }
        public string? extra { get; set; }
        public int? port { get; set; }
    }

    public class Databases
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public class Tables
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }
}
