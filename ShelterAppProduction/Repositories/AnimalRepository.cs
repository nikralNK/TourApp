using Npgsql;
using ShelterAppProduction.Database;
using ShelterAppProduction.Models;
using System;
using System.Collections.Generic;

namespace ShelterAppProduction.Repositories
{
    public class AnimalRepository
    {
        public List<Animal> GetAll()
        {
            var animals = new List<Animal>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = "SELECT * FROM Animal";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            animals.Add(MapAnimal(reader));
                        }
                    }
                }
            }
            catch { }
            return animals;
        }

        public List<Animal> GetFiltered(string type = null, string gender = null, string size = null, int? minAge = null, int? maxAge = null)
        {
            var animals = new List<Animal>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = "SELECT * FROM Animal WHERE 1=1";

                    if (!string.IsNullOrEmpty(type))
                        query += " AND Type = @type";
                    if (!string.IsNullOrEmpty(gender))
                        query += " AND Gender = @gender";
                    if (!string.IsNullOrEmpty(size))
                        query += " AND Size = @size";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(type))
                            cmd.Parameters.AddWithValue("@type", type);
                        if (!string.IsNullOrEmpty(gender))
                            cmd.Parameters.AddWithValue("@gender", gender);
                        if (!string.IsNullOrEmpty(size))
                            cmd.Parameters.AddWithValue("@size", size);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var animal = MapAnimal(reader);
                                if (minAge.HasValue && animal.Age < minAge.Value)
                                    continue;
                                if (maxAge.HasValue && animal.Age > maxAge.Value)
                                    continue;
                                animals.Add(animal);
                            }
                        }
                    }
                }
            }
            catch { }
            return animals;
        }

        public Animal GetById(int id)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = "SELECT * FROM Animal WHERE Id = @id";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapAnimal(reader);
                            }
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        private Animal MapAnimal(NpgsqlDataReader reader)
        {
            return new Animal
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Type = reader.IsDBNull(2) ? null : reader.GetString(2),
                Breed = reader.IsDBNull(3) ? null : reader.GetString(3),
                DateOfBirth = reader.IsDBNull(4) ? DateTime.Today : reader.GetDateTime(4),
                IdEnclosure = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                IdGuardian = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                CurrentStatus = reader.IsDBNull(7) ? "Available" : reader.GetString(7),
                Gender = reader.IsDBNull(8) ? null : reader.GetString(8),
                Size = reader.IsDBNull(9) ? null : reader.GetString(9),
                Temperament = reader.IsDBNull(10) ? null : reader.GetString(10)
            };
        }
    }
}
