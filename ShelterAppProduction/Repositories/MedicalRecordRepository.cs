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
                    var query = "SELECT * FROM medical_record WHERE idanimal = @animalId ORDER BY visitdate DESC";
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
                    var query = "SELECT fullname FROM veterinarian WHERE id = @id";
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

        public bool AddMedicalRecord(MedicalRecord record, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = @"INSERT INTO medical_record (idanimal, idveterinarian, visitdate, diagnosis, treatment, notes)
                                  VALUES (@idAnimal, @idVeterinarian, @visitDate, @diagnosis, @treatment, @notes)";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idAnimal", record.IdAnimal);
                        cmd.Parameters.AddWithValue("@idVeterinarian", record.IdVeterinarian);
                        cmd.Parameters.AddWithValue("@visitDate", record.VisitDate);
                        cmd.Parameters.AddWithValue("@diagnosis", record.Diagnosis ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@treatment", record.Treatment ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@notes", record.Notes ?? (object)DBNull.Value);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public int? GetVeterinarianIdByUserId(int userId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = "SELECT id FROM veterinarian WHERE iduser = @userId";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        var result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : (int?)null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
