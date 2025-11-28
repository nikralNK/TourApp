using System;

namespace ShelterAppProduction.Models
{
    public class Animal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Breed { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? IdEnclosure { get; set; }
        public int? IdGuardian { get; set; }
        public string CurrentStatus { get; set; }
        public string Gender { get; set; }
        public string Size { get; set; }
        public string Temperament { get; set; }

        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
    }
}
