using BCrypt.Net;
using Npgsql;
using ShelterAppProduction.Database;
using ShelterAppProduction.Models;

namespace ShelterAppProduction.Services
{
    public class AuthService
    {
        public User Login(string username, string password)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = "SELECT * FROM Users WHERE Username = @username";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var user = new User
                                {
                                    Id = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    PasswordHash = reader.GetString(2),
                                    Email = reader.GetString(3),
                                    FullName = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    Role = reader.IsDBNull(5) ? "User" : reader.GetString(5)
                                };

                                if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                                {
                                    SessionManager.CurrentUser = user;
                                    return user;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public void ResetPassword(string username, string newPassword)
        {
            try
            {
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = "UPDATE Users SET PasswordHash = @passwordHash WHERE Username = @username";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }
    }
}
