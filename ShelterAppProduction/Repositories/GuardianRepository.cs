using Npgsql;
using ShelterAppProduction.Database;
using ShelterAppProduction.Models;
using System;

namespace ShelterAppProduction.Repositories
{
    public class GuardianRepository
    {
        public Guardian GetById(int id)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = "SELECT * FROM Guardian WHERE Id = @id";
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
        }
    }
}
