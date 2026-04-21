using System.Data;
using Microsoft.Data.SqlClient;

namespace Stockify.Data
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        /// <summary>
        /// Vérifie les identifiants. Retourne le UserID si succès, -1 si échec.
        /// </summary>
        public async Task<int> Login(string email, string password)
        {
            string query = "SELECT UserID, PasswordHashed FROM Users WHERE Email = @email";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;

                try
                {
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string storedHash = reader["PasswordHashed"].ToString();
                            int userId = (int)reader["UserID"];

                            if (storedHash.Trim() == password.Trim())
                                return userId;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Login] Erreur: {ex.Message}");
                }
            }
            return -1;
        }

        /// <summary>
        /// Crée un nouveau compte. Retourne (true, message) si succès.
        /// </summary>
        public async Task<(bool success, string message)> Register(string name, string email, string password)
        {
            string checkQuery = "SELECT COUNT(*) FROM Users WHERE Email = @email";
            string insertQuery = "INSERT INTO Users (Name, Email, PasswordHashed, Balance) VALUES (@name, @email, @password, 0)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    Console.WriteLine($"[Register] Connecté à: {conn.DataSource} / {conn.Database}");

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;
                        int count = (int)await checkCmd.ExecuteScalarAsync();
                        Console.WriteLine($"[Register] Count pour {email}: {count}");
                        if (count > 0) return (false, "Cet email est déjà utilisé.");
                    }

                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                        insertCmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;
                        insertCmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;

                        int rows = await insertCmd.ExecuteNonQueryAsync();
                        return rows > 0
                            ? (true, "Compte créé avec succès!")
                            : (false, "Erreur lors de la création du compte.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Register] Erreur: {ex.Message}");
                    return (false, $"Erreur: {ex.Message}");
                }
            }
        }
    }
}