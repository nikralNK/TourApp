using System;

namespace ShelterAppProduction.Models
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        public int IdAnimal { get; set; }
        public int IdVeterinarian { get; set; }
        public DateTime VisitDate { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Notes { get; set; }
    }
}
