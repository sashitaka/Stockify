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

        public async Task<(bool success, string message)> Register(string name, string email, string password)
        {
            string checkQuery = "SELECT COUNT(*) FROM Users WHERE Email = @email";
            string insertQuery = "INSERT INTO Users (Name, Email, PasswordHashed, Balance) VALUES (@name, @email, @password, 0)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;
                        int count = (int)await checkCmd.ExecuteScalarAsync();
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

        public async Task<string> GetUserName(int userId)
        {
            string query = "SELECT Name FROM Users WHERE UserID = @userId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                try
                {
                    await conn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    return result?.ToString() ?? "";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GetUserName] Erreur: {ex.Message}");
                    return "";
                }
            }
        }

        public async Task<string> GetEmail(int userId)
        {
            string query = "SELECT Email FROM Users WHERE UserID = @userId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                try
                {
                    await conn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    return result?.ToString() ?? "";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GetEmail] Erreur: {ex.Message}");
                    return "";
                }
            }
        }

        public async Task<string> GetPassword(int userId)
        {
            string query = "SELECT PasswordHashed FROM Users WHERE UserID = @userId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                try
                {
                    await conn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    return result?.ToString() ?? "";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GetPassword] Erreur: {ex.Message}");
                    return "";
                }
            }
        }


        public async Task<decimal> GetBalance(int userId)
        {
            string query = "SELECT Balance FROM Users WHERE UserID = @userId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                try
                {
                    await conn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    return result != null ? (decimal)result : 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GetBalance] Erreur: {ex.Message}");
                    return 0;
                }
            }
        }

        public async Task<bool> UpdateBalance(int userId, decimal newBalance)
        {
            string query = "UPDATE Users SET Balance = @balance WHERE UserID = @userId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@balance", SqlDbType.Decimal).Value = newBalance;
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                try
                {
                    await conn.OpenAsync();
                    int rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[UpdateBalance] Erreur: {ex.Message}");
                    return false;
                }
            }
        }

        public async Task<bool> UpdateInfos(int userId, string newEmail, string newPassword, decimal newBalance)
        {
            string query = "UPDATE Users SET Email = @Email," +
                " PasswordHashed = @Password," +
                " Balance = @Balance" +
                " WHERE UserID = @userId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = newEmail;
                cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = newPassword;
                cmd.Parameters.Add("@Balance", SqlDbType.Decimal).Value = newBalance;
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                try
                {
                    await conn.OpenAsync();
                    int rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[UpdateInfos] Erreur: {ex.Message}");
                    return false;
                }
            }
        }


        /// <summary>
        /// Achète une quantité de stock. Utilise StockName directement.
        /// </summary>
        public async Task<(bool success, string message)> BuyStock(int userId, string ticker, decimal quantity, decimal totalCost)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Vérifier le solde
                        decimal balance;
                        using (SqlCommand balCmd = new SqlCommand("SELECT Balance FROM Users WHERE UserID = @userId", conn, transaction))
                        {
                            balCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                            var result = await balCmd.ExecuteScalarAsync();
                            balance = result != null ? (decimal)result : 0;
                        }

                        if (balance < totalCost)
                            return (false, "Solde insuffisant.");

                        // Vérifier si le stock existe déjà dans le portfolio
                        using (SqlCommand checkCmd = new SqlCommand("SELECT Quantity FROM PortfolioItems WHERE UserID = @userId AND StockName = @ticker", conn, transaction))
                        {
                            checkCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                            checkCmd.Parameters.Add("@ticker", SqlDbType.NVarChar).Value = ticker;
                            var existing = await checkCmd.ExecuteScalarAsync();

                            if (existing != null)
                            {
                                using (SqlCommand updateCmd = new SqlCommand("UPDATE PortfolioItems SET Quantity = Quantity + @qty WHERE UserID = @userId AND StockName = @ticker", conn, transaction))
                                {
                                    updateCmd.Parameters.Add("@qty", SqlDbType.Decimal).Value = quantity;
                                    updateCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                                    updateCmd.Parameters.Add("@ticker", SqlDbType.NVarChar).Value = ticker;
                                    await updateCmd.ExecuteNonQueryAsync();
                                }
                            }
                            else
                            {
                                using (SqlCommand insertCmd = new SqlCommand("INSERT INTO PortfolioItems (UserID, StockName, Quantity) VALUES (@userId, @ticker, @qty)", conn, transaction))
                                {
                                    insertCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                                    insertCmd.Parameters.Add("@ticker", SqlDbType.NVarChar).Value = ticker;
                                    insertCmd.Parameters.Add("@qty", SqlDbType.Decimal).Value = quantity;
                                    await insertCmd.ExecuteNonQueryAsync();
                                }
                            }
                        }

                        // Déduire le solde
                        using (SqlCommand updateBal = new SqlCommand("UPDATE Users SET Balance = Balance - @cost WHERE UserID = @userId", conn, transaction))
                        {
                            updateBal.Parameters.Add("@cost", SqlDbType.Decimal).Value = totalCost;
                            updateBal.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                            await updateBal.ExecuteNonQueryAsync();
                        }

                        // Enregistrer la transaction avec StockName
                        using (SqlCommand transCmd = new SqlCommand("INSERT INTO Transactions (UserID, StockName, TransacType, Quantity, TransacDate) VALUES (@userId, @ticker, 'BUY', @qty, @date)", conn, transaction))
                        {
                            transCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                            transCmd.Parameters.Add("@ticker", SqlDbType.NVarChar).Value = ticker;
                            transCmd.Parameters.Add("@qty", SqlDbType.Decimal).Value = quantity;
                            transCmd.Parameters.Add("@date", SqlDbType.Date).Value = DateTime.Today;
                            await transCmd.ExecuteNonQueryAsync();
                        }

                        await transaction.CommitAsync();
                        return (true, "Achat effectué avec succès!");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine($"[BuyStock] Erreur: {ex.Message}");
                        return (false, $"Erreur: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Vend une quantité de stock. Utilise StockName directement.
        /// </summary>
        public async Task<(bool success, string message)> SellStock(int userId, string ticker, decimal quantity, decimal totalGain)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Vérifier la quantité disponible
                        decimal currentQty;
                        using (SqlCommand checkCmd = new SqlCommand("SELECT Quantity FROM PortfolioItems WHERE UserID = @userId AND StockName = @ticker", conn, transaction))
                        {
                            checkCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                            checkCmd.Parameters.Add("@ticker", SqlDbType.NVarChar).Value = ticker;
                            var result = await checkCmd.ExecuteScalarAsync();
                            if (result == null) return (false, "Vous ne possédez pas ce stock.");
                            currentQty = (decimal)result;
                        }

                        if (currentQty < quantity)
                            return (false, $"Quantité insuffisante. Vous possédez {currentQty:F6} actions.");

                        // Mettre à jour ou supprimer du portfolio
                        if (currentQty == quantity)
                        {
                            using (SqlCommand deleteCmd = new SqlCommand("DELETE FROM PortfolioItems WHERE UserID = @userId AND StockName = @ticker", conn, transaction))
                            {
                                deleteCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                                deleteCmd.Parameters.Add("@ticker", SqlDbType.NVarChar).Value = ticker;
                                await deleteCmd.ExecuteNonQueryAsync();
                            }
                        }
                        else
                        {
                            using (SqlCommand updateCmd = new SqlCommand("UPDATE PortfolioItems SET Quantity = Quantity - @qty WHERE UserID = @userId AND StockName = @ticker", conn, transaction))
                            {
                                updateCmd.Parameters.Add("@qty", SqlDbType.Decimal).Value = quantity;
                                updateCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                                updateCmd.Parameters.Add("@ticker", SqlDbType.NVarChar).Value = ticker;
                                await updateCmd.ExecuteNonQueryAsync();
                            }
                        }

                        // Ajouter le gain au solde
                        using (SqlCommand updateBal = new SqlCommand("UPDATE Users SET Balance = Balance + @gain WHERE UserID = @userId", conn, transaction))
                        {
                            updateBal.Parameters.Add("@gain", SqlDbType.Decimal).Value = totalGain;
                            updateBal.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                            await updateBal.ExecuteNonQueryAsync();
                        }

                        // Enregistrer la transaction avec StockName
                        using (SqlCommand transCmd = new SqlCommand("INSERT INTO Transactions (UserID, StockName, TransacType, Quantity, TransacDate) VALUES (@userId, @ticker, 'SELL', @qty, @date)", conn, transaction))
                        {
                            transCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                            transCmd.Parameters.Add("@ticker", SqlDbType.NVarChar).Value = ticker;
                            transCmd.Parameters.Add("@qty", SqlDbType.Decimal).Value = quantity;
                            transCmd.Parameters.Add("@date", SqlDbType.Date).Value = DateTime.Today;
                            await transCmd.ExecuteNonQueryAsync();
                        }

                        await transaction.CommitAsync();
                        return (true, "Vente effectuée avec succès!");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine($"[SellStock] Erreur: {ex.Message}");
                        return (false, $"Erreur: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Retourne le portfolio d'un utilisateur.
        /// </summary>
        public async Task<List<(string StockName, decimal Quantity)>> GetPortfolio(int userId)
        {
            var portfolio = new List<(string StockName, decimal Quantity)>();
            string query = "SELECT StockName, Quantity FROM PortfolioItems WHERE UserID = @userId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                try
                {
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string name = reader["StockName"]?.ToString() ?? string.Empty;
                            decimal qty = (decimal)reader["Quantity"];
                            portfolio.Add((name, qty));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GetPortfolio] Erreur: {ex.Message}");
                }
            }
            return portfolio;
        }

        /// <summary>
        /// Retourne l'historique des transactions d'un utilisateur.
        /// </summary>
        public async Task<List<(int TransactionID, string StockName, string Type, decimal Quantity, DateTime Date)>> GetTransactionHistory(int userId)
        {
            var history = new List<(int, string, string, decimal, DateTime)>();
            string query = @"SELECT TransactionID, StockName, TransacType, Quantity, TransacDate
                             FROM Transactions
                             WHERE UserID = @userId
                             ORDER BY TransacDate DESC";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                try
                {
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            history.Add((
                                (int)reader["TransactionID"],
                                reader["StockName"].ToString(),
                                reader["TransacType"].ToString(),
                                (decimal)reader["Quantity"],
                                (DateTime)reader["TransacDate"]
                            ));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GetTransactionHistory] Erreur: {ex.Message}");
                }
            }
            return history;
        }

        /// <summary>
        /// Retourne la watchlist d'un utilisateur.
        /// </summary>
        public async Task<List<string>> GetWatchlist(int userId)
        {
            var watchlist = new List<string>();
            string query = "SELECT StockName FROM Watchlist WHERE UserID = @userId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                try
                {
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            watchlist.Add(reader["StockName"].ToString()!);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GetWatchlist] Erreur: {ex.Message}");
                }
            }
            return watchlist;
        }

        /// <summary>
        /// Ajoute un stock à la watchlist.
        /// </summary>
        public async Task<(bool success, string message)> AddToWatchlist(int userId, string ticker)
        {
            string checkQuery = "SELECT COUNT(*) FROM Watchlist WHERE UserID = @userId AND StockName = @ticker";
            string insertQuery = "INSERT INTO Watchlist (UserID, StockName) VALUES (@userId, @ticker)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                        checkCmd.Parameters.Add("@ticker", SqlDbType.NVarChar).Value = ticker;
                        int count = (int)await checkCmd.ExecuteScalarAsync();
                        if (count > 0) return (false, "Stock déjà dans la watchlist.");
                    }

                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                        insertCmd.Parameters.Add("@ticker", SqlDbType.NVarChar).Value = ticker;
                        int rows = await insertCmd.ExecuteNonQueryAsync();
                        return rows > 0
                            ? (true, "Stock ajouté à la watchlist!")
                            : (false, "Erreur lors de l'ajout.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AddToWatchlist] Erreur: {ex.Message}");
                    return (false, $"Erreur: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Supprime un stock de la watchlist.
        /// </summary>
        public async Task<bool> RemoveFromWatchlist(int userId, string ticker)
        {
            string query = "DELETE FROM Watchlist WHERE UserID = @userId AND StockName = @ticker";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@ticker", SqlDbType.NVarChar).Value = ticker;
                try
                {
                    await conn.OpenAsync();
                    int rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RemoveFromWatchlist] Erreur: {ex.Message}");
                    return false;
                }
            }
        }



    }
}