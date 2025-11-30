using System;

namespace ShelterAppProduction.Models
{
    public class Veterinarian
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }
        public string PhoneNumber { get; set; }
        public string LicenseNumber { get; set; }
        public int? UserId { get; set; }
    }
}
