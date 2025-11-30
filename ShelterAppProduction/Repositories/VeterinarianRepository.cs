using Npgsql;
using ShelterAppProduction.Database;
using ShelterAppProduction.Models;
using System;
using System.Collections.Generic;

namespace ShelterAppProduction.Repositories
{
    public class VeterinarianRepository
    {
        public List<Veterinarian> GetAll()
        {
            var veterinarians = new List<Veterinarian>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = "SELECT * FROM Veterinarian";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            veterinarians.Add(new Veterinarian
                            {
                                Id = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                Specialization = reader.IsDBNull(2) ? null : reader.GetString(2),
                                PhoneNumber = reader.IsDBNull(3) ? null : reader.GetString(3),
                                LicenseNumber = reader.IsDBNull(4) ? null : reader.GetString(4),
                                UserId = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5)
                            });
                        }
                    }
                }
            }
            catch { }
            return veterinarians;
        }

        public Veterinarian GetById(int id)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = "SELECT * FROM Veterinarian WHERE Id = @id";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Veterinarian
                                {
                                    Id = reader.GetInt32(0),
                                    FullName = reader.GetString(1),
                                    Specialization = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    PhoneNumber = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    LicenseNumber = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    UserId = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5)
                                };
                            }
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public bool AddVeterinarian(Veterinarian veterinarian)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = @"INSERT INTO Veterinarian (FullName, Specialization, PhoneNumber, LicenseNumber, UserId)
                                  VALUES (@fullName, @specialization, @phoneNumber, @licenseNumber, @userId)";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@fullName", veterinarian.FullName);
                        cmd.Parameters.AddWithValue("@specialization", veterinarian.Specialization ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@phoneNumber", veterinarian.PhoneNumber ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@licenseNumber", veterinarian.LicenseNumber ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@userId", veterinarian.UserId.HasValue ? (object)veterinarian.UserId.Value : DBNull.Value);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateVeterinarian(Veterinarian veterinarian)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = @"UPDATE Veterinarian SET FullName = @fullName, Specialization = @specialization,
                                  PhoneNumber = @phoneNumber, LicenseNumber = @licenseNumber, UserId = @userId WHERE Id = @id";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", veterinarian.Id);
                        cmd.Parameters.AddWithValue("@fullName", veterinarian.FullName);
                        cmd.Parameters.AddWithValue("@specialization", veterinarian.Specialization ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@phoneNumber", veterinarian.PhoneNumber ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@licenseNumber", veterinarian.LicenseNumber ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@userId", veterinarian.UserId.HasValue ? (object)veterinarian.UserId.Value : DBNull.Value);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteVeterinarian(int id)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = "DELETE FROM Veterinarian WHERE Id = @id";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
