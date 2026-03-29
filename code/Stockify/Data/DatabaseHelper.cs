using System.Data;
using System.Runtime.InteropServices;
using Microsoft.Data.SqlClient;

namespace Stockify.Data
{
    public class DatabaseHelper
    {
        private readonly string _connectionString = "Server=tcp:servercedrick.database.windows.net,1433;Initial Catalog=Stockify;Persist Security Info=False;User ID=CloudSA3aca7e4d;Password=Monchienmao1996;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<Boolean> Login(string email, string password)
        {
            string query = "SELECT PasswordHash FROM Users WHERE Email = @email";

            using(SqlConnection conn = new SqlConnection(_connectionString))
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
                            string storedPassword = result.ToString();

                            return storedPassword == password;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                    
                }
            }

            return false;
        }
    }
}