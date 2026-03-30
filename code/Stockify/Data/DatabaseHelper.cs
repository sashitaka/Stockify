using System.Data;
using System.Runtime.InteropServices;
using Microsoft.Data.SqlClient;

namespace Stockify.Data
{
    public class DatabaseHelper
    {
        private readonly string _connectionString = "Server=tcp:servercedrick.database.windows.net,1433;Initial Catalog=Stockify;Persist Security Info=False;User ID=CloudSA3aca7e4d;Password={Monchienmao1996};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<(bool success, string message)> TestConnection()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    return (true, "Connexion réussie!");
                }
                catch (SqlException ex)
                {
                    // This will capture the specific Azure error (like IP Blocked)
                    return (false, $"Erreur SQL: {ex.Message}");
                }
                catch (Exception ex)
                {
                    return (false, $"Erreur générale: {ex.Message}");
                }
            }
        }

        public async Task<bool> Login(string email, string password)
        {
            // Updated to use the 'PasswordHash' column name from your screenshot
            string query = "SELECT PasswordHash FROM Users WHERE Email = @email";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;

                    try
                    {
                        await conn.OpenAsync();
                        var result = await cmd.ExecuteScalarAsync();

                        if (result != null)
                        {
                            string storedHash = result.ToString();

                            // Note: If you stored the password as plain text in 'PasswordHash', 
                            // this direct comparison will work. 
                            // If you hashed it, you'll need BCrypt.Verify(password, storedHash).
                            return storedHash.Trim() == password.Trim();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Basic error handling
                        return false;
                    }
                }
            }
            return false;
        }
    }
}