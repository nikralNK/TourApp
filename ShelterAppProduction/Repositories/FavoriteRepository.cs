using Npgsql;
using ShelterAppProduction.Database;
using ShelterAppProduction.Models;
using System;
using System.Collections.Generic;

namespace ShelterAppProduction.Repositories
{
    public class FavoriteRepository
    {
        public List<Animal> GetFavoritesByUser(int userId)
        {
            var animals = new List<Animal>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = @"SELECT a.* FROM Animal a
                                  INNER JOIN Favorite f ON a.Id = f.IdAnimal
                                  WHERE f.IdUser = @userId";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                animals.Add(new Animal
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Type = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    Breed = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    DateOfBirth = reader.IsDBNull(4) ? DateTime.Today : reader.GetDateTime(4),
                                    IdEnclosure = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                                    IdGuardian = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                                    CurrentStatus = reader.IsDBNull(7) ? "Доступен" : reader.GetString(7),
                                    Gender = reader.IsDBNull(8) ? null : reader.GetString(8),
                                    Size = reader.IsDBNull(9) ? null : reader.GetString(9),
                                    Temperament = reader.IsDBNull(10) ? null : reader.GetString(10)
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return animals;
        }

        public void AddFavorite(int userId, int animalId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = "INSERT INTO Favorite (IdUser, IdAnimal, AddedDate) VALUES (@userId, @animalId, @date) ON CONFLICT DO NOTHING";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@animalId", animalId);
                        cmd.Parameters.AddWithValue("@date", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }

        public void RemoveFavorite(int userId, int animalId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = "DELETE FROM Favorite WHERE IdUser = @userId AND IdAnimal = @animalId";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@animalId", animalId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }

        public bool IsFavorite(int userId, int animalId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    var query = "SELECT COUNT(*) FROM Favorite WHERE IdUser = @userId AND IdAnimal = @animalId";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@animalId", animalId);
                        var count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch { }
            return false;
        }
    }
}
