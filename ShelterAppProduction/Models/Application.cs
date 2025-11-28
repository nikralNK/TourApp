using System;

namespace ShelterAppProduction.Models
{
    public class Application
    {
        public int Id { get; set; }
        public int IdAnimal { get; set; }
        public int IdGuardian { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
    }
}
