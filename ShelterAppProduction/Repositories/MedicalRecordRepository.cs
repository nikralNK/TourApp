using Npgsql;
using ShelterAppProduction.Database;
using ShelterAppProduction.Models;
using System;
using System.Collections.Generic;

namespace ShelterAppProduction.Repositories
{
    public class MedicalRecordRepository
    {
        public List<MedicalRecord> GetByAnimalId(int animalId)
        {
            var records = new List<MedicalRecord>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = "SELECT * FROM MedicalRecord WHERE IdAnimal = @animalId ORDER BY VisitDate DESC";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@animalId", animalId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                records.Add(new MedicalRecord
                                {
                                    Id = reader.GetInt32(0),
                                    IdAnimal = reader.GetInt32(1),
                                    IdVeterinarian = reader.GetInt32(2),
                                    VisitDate = reader.GetDateTime(3),
                                    Diagnosis = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    Treatment = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    Notes = reader.IsDBNull(6) ? null : reader.GetString(6)
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return records;
        }

        public string GetVeterinarianName(int veterinarianId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = "SELECT FullName FROM Veterinarian WHERE Id = @id";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", veterinarianId);
                        var result = cmd.ExecuteScalar();
                        return result?.ToString() ?? "Не указан";
                    }
                }
            }
            catch
            {
                return "Не указан";
            }
        }
    }
}
