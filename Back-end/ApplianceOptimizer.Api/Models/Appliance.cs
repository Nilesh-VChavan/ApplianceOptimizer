namespace ApplianceOptimizer.Api.Models
{
    public class Appliance
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public int Wattage { get; set; }

        public int Hours { get; set; }

        public string Category { get; set; } = "";

        // Foreign key
        public int UserId { get; set; }

        public User? User { get; set; }

    }
}
