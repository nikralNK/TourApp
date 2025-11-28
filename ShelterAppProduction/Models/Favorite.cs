using System;

namespace ShelterAppProduction.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public int IdAnimal { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
