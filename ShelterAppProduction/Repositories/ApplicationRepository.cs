using Npgsql;
using ShelterAppProduction.Database;
using ShelterAppProduction.Models;
using System;
using System.Collections.Generic;

namespace ShelterAppProduction.Repositories
{
    public class ApplicationRepository
    {
        public void CreateApplication(int animalId, string guardianFullName, string guardianPhone, string guardianEmail, string comments)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    int guardianId;
                    var checkGuardian = "SELECT Id FROM Guardian WHERE Email = @email";
                    using (var cmd = new NpgsqlCommand(checkGuardian, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", guardianEmail);
                        var result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            guardianId = Convert.ToInt32(result);
                        }
                        else
                        {
                            var insertGuardian = "INSERT INTO Guardian (FullName, PhoneNumber, Email, RegistrationDate) VALUES (@fullName, @phone, @email, @date) RETURNING Id";
                            using (var insertCmd = new NpgsqlCommand(insertGuardian, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@fullName", guardianFullName);
                                insertCmd.Parameters.AddWithValue("@phone", guardianPhone);
                                insertCmd.Parameters.AddWithValue("@email", guardianEmail);
                                insertCmd.Parameters.AddWithValue("@date", DateTime.Now);
                                guardianId = Convert.ToInt32(insertCmd.ExecuteScalar());
                            }
                        }
                    }

                    var insertApplication = "INSERT INTO Application (IdAnimal, IdGuardian, ApplicationDate, Status, Comments) VALUES (@animalId, @guardianId, @date, @status, @comments)";
                    using (var cmd = new NpgsqlCommand(insertApplication, conn))
                    {
                        cmd.Parameters.AddWithValue("@animalId", animalId);
                        cmd.Parameters.AddWithValue("@guardianId", guardianId);
                        cmd.Parameters.AddWithValue("@date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@status", "На рассмотрении");
                        cmd.Parameters.AddWithValue("@comments", comments ?? "");
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }

        public List<Application> GetAllApplications()
        {
            var applications = new List<Application>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = @"SELECT a.Id, a.IdAnimal, a.IdGuardian, a.ApplicationDate, a.Status, a.Comments, g.FullName
                                  FROM Application a
                                  LEFT JOIN Guardian g ON a.IdGuardian = g.Id
                                  ORDER BY a.ApplicationDate DESC";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var fullName = reader.IsDBNull(6) ? "" : reader.GetString(6);
                            var shortName = GetShortName(fullName);

                            applications.Add(new Application
                            {
                                Id = reader.GetInt32(0),
                                IdAnimal = reader.GetInt32(1),
                                IdGuardian = reader.GetInt32(2),
                                ApplicationDate = reader.GetDateTime(3),
                                Status = reader.IsDBNull(4) ? "На рассмотрении" : reader.GetString(4),
                                Comments = reader.IsDBNull(5) ? null : reader.GetString(5),
                                GuardianShortName = shortName
                            });
                        }
                    }
                }
            }
            catch { }
            return applications;
        }

        private string GetShortName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "Не указано";

            var parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return "Не указано";
            if (parts.Length == 1)
                return parts[0];
            if (parts.Length == 2)
                return $"{parts[0]} {parts[1][0]}.";

            return $"{parts[0]} {parts[1][0]}. {parts[2][0]}.";
        }

        public void UpdateApplicationStatus(int applicationId, string status)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = "UPDATE Application SET Status = @status WHERE Id = @id";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@id", applicationId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }
    }
}
