using System;

namespace ShelterAppProduction.Models
{
    public class ShelterEmployee
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime HireDate { get; set; }
    }
}
