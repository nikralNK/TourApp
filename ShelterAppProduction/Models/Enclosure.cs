using System;

namespace ShelterAppProduction.Models
{
    public class Enclosure
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Capacity { get; set; }
        public string Location { get; set; }
    }
}
