using BCrypt.Net;
using Npgsql;
using ShelterAppProduction.Database;
using ShelterAppProduction.Models;
using System;
using System.Windows;

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
                                    Role = reader.IsDBNull(5) ? "User" : reader.GetString(5),
                                    Avatar = reader.IsDBNull(6) ? null : reader.GetString(6)
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при входе: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сбросе пароля: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool Register(string username, string password, string email, string fullName)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    var checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                    using (var checkCmd = new NpgsqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@username", username);
                        var count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            return false;
                        }
                    }

                    var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
                    var insertQuery = "INSERT INTO Users (Username, PasswordHash, Email, FullName, Role) VALUES (@username, @passwordHash, @email, @fullName, @role)";
                    using (var cmd = new NpgsqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@fullName", fullName);
                        cmd.Parameters.AddWithValue("@role", "User");
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool UpdateProfile(int userId, string fullName, string avatar)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = "UPDATE Users SET FullName = @fullName, Avatar = @avatar WHERE Id = @userId";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@fullName", fullName);
                        cmd.Parameters.AddWithValue("@avatar", avatar ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.ExecuteNonQuery();

                        if (SessionManager.CurrentUser != null && SessionManager.CurrentUser.Id == userId)
                        {
                            SessionManager.CurrentUser.FullName = fullName;
                            SessionManager.CurrentUser.Avatar = avatar;
                        }

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении профиля: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
