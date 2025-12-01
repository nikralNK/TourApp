using Npgsql;
using ShelterAppProduction.Database;
using ShelterAppProduction.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShelterAppProduction.Repositories
{
    public class GuardianRepository
    {
        public async Task<Guardian> GetById(int id)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (var conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();
                        var query = "SELECT * FROM guardian WHERE id = @id";
                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    return new Guardian
                                    {
                                        Id = reader.GetInt32(0),
                                        FullName = reader.GetString(1),
                                        PhoneNumber = reader.IsDBNull(2) ? null : reader.GetString(2),
                                        Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                                        Address = reader.IsDBNull(4) ? null : reader.GetString(4),
                                        RegistrationDate = reader.IsDBNull(5) ? DateTime.Now : reader.GetDateTime(5)
                                    };
                                }
                            }
                        }
                    }
                }
                catch { }
                return null;
            });
        }

        public async Task<List<Guardian>> GetAllGuardians()
        {
            return await Task.Run(() =>
            {
                var guardians = new List<Guardian>();
                try
                {
                    using (var conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();
                        var query = "SELECT * FROM guardian";
                        using (var cmd = new NpgsqlCommand(query, conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                guardians.Add(new Guardian
                                {
                                    Id = reader.GetInt32(0),
                                    FullName = reader.GetString(1),
                                    PhoneNumber = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    Address = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    RegistrationDate = reader.IsDBNull(5) ? DateTime.Now : reader.GetDateTime(5)
                                });
                            }
                        }
                    }
                }
                catch { }
                return guardians;
            });
        }

        public async Task<int> GetOrCreateGuardian(string fullName, string phoneNumber, string email)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (var conn = DatabaseHelper.GetConnection())
                    {
                        conn.Open();

                        var checkQuery = "SELECT id FROM guardian WHERE email = @email";
                        using (var cmd = new NpgsqlCommand(checkQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@email", email);
                            var result = cmd.ExecuteScalar();

                            if (result != null)
                            {
                                return Convert.ToInt32(result);
                            }
                        }

                        var insertQuery = "INSERT INTO guardian (fullname, phonenumber, email, registrationdate) VALUES (@fullName, @phone, @email, @date) RETURNING id";
                        using (var insertCmd = new NpgsqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@fullName", fullName);
                            insertCmd.Parameters.AddWithValue("@phone", phoneNumber ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@email", email);
                            insertCmd.Parameters.AddWithValue("@date", DateTime.Now);
                            return Convert.ToInt32(insertCmd.ExecuteScalar());
                        }
                    }
                }
                catch
                {
                    return 0;
                }
            });
        }
    }
}
