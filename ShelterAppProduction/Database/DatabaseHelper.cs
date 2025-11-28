using Npgsql;

namespace ShelterAppProduction.Database
{
    public static class DatabaseHelper
    {
        private static string connectionString = "Host=localhost;Port=5432;Database=shelter_db;Username=postgres;Password=postgres";

        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}
