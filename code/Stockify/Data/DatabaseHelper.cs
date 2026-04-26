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

        /// <summary>
        /// Retourne le nom d'un utilisateur.
        /// </summary>
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

        /// <summary>
        /// Retourne le solde d'un utilisateur.
        /// </summary>
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

        /// <summary>
        /// Met à jour le solde d'un utilisateur.
        /// </summary>
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

        /// <summary>
        /// Achète une quantité de stock pour un utilisateur.
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

                        // Récupérer ou créer le stock
                        int stockId;
                        using (SqlCommand stockCmd = new SqlCommand("SELECT StockID FROM Stocks WHERE StockName = @ticker", conn, transaction))
                        {
                            stockCmd.Parameters.Add("@ticker", SqlDbType.NVarChar).Value = ticker;
                            var result = await stockCmd.ExecuteScalarAsync();

                            if (result != null)
                            {
                                stockId = (int)result;
                            }
                            else
                            {
                                using (SqlCommand insertStock = new SqlCommand("INSERT INTO Stocks (StockName) OUTPUT INSERTED.StockID VALUES (@ticker)", conn, transaction))
                                {
                                    insertStock.Parameters.Add("@ticker", SqlDbType.NVarChar).Value = ticker;
                                    stockId = (int)await insertStock.ExecuteScalarAsync();
                                }
                            }
                        }

                        // Mettre à jour le portfolio
                        using (SqlCommand checkCmd = new SqlCommand("SELECT Quantity FROM PortfolioItems WHERE UserID = @userId AND StockID = @stockId", conn, transaction))
                        {
                            checkCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                            checkCmd.Parameters.Add("@stockId", SqlDbType.Int).Value = stockId;
                            var existing = await checkCmd.ExecuteScalarAsync();

                            if (existing != null)
                            {
                                using (SqlCommand updateCmd = new SqlCommand("UPDATE PortfolioItems SET Quantity = Quantity + @qty WHERE UserID = @userId AND StockID = @stockId", conn, transaction))
                                {
                                    updateCmd.Parameters.Add("@qty", SqlDbType.Decimal).Value = quantity;
                                    updateCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                                    updateCmd.Parameters.Add("@stockId", SqlDbType.Int).Value = stockId;
                                    await updateCmd.ExecuteNonQueryAsync();
                                }
                            }
                            else
                            {
                                using (SqlCommand insertCmd = new SqlCommand("INSERT INTO PortfolioItems (UserID, StockID, Quantity) VALUES (@userId, @stockId, @qty)", conn, transaction))
                                {
                                    insertCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                                    insertCmd.Parameters.Add("@stockId", SqlDbType.Int).Value = stockId;
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

                        // Enregistrer la transaction
                        using (SqlCommand transCmd = new SqlCommand("INSERT INTO Transactions (UserID, StockID, TransacType, Quantity, TransacDate) VALUES (@userId, @stockId, 'BUY', @qty, @date)", conn, transaction))
                        {
                            transCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                            transCmd.Parameters.Add("@stockId", SqlDbType.Int).Value = stockId;
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
        /// Vend une quantité de stock pour un utilisateur.
        /// </summary>
        public async Task<(bool success, string message)> SellStock(int userId, string ticker, float quantity, float totalGain)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Récupérer le StockID
                        int stockId;
                        using (SqlCommand stockCmd = new SqlCommand("SELECT StockID FROM Stocks WHERE StockName = @ticker", conn, transaction))
                        {
                            stockCmd.Parameters.Add("@ticker", SqlDbType.NVarChar).Value = ticker;
                            var result = await stockCmd.ExecuteScalarAsync();
                            if (result == null) return (false, "Stock introuvable.");
                            stockId = (int)result;
                        }

                        // Vérifier la quantité disponible
                        float currentQty;
                        using (SqlCommand checkCmd = new SqlCommand("SELECT Quantity FROM PortfolioItems WHERE UserID = @userId AND StockID = @stockId", conn, transaction))
                        {
                            checkCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                            checkCmd.Parameters.Add("@stockId", SqlDbType.Int).Value = stockId;
                            var result = await checkCmd.ExecuteScalarAsync();
                            if (result == null) return (false, "Vous ne possédez pas ce stock.");
                            currentQty = (float)result;
                        }

                        if (currentQty < quantity)
                            return (false, $"Quantité insuffisante. Vous possédez {currentQty:F6} actions.");

                        // Mettre à jour ou supprimer du portfolio
                        if (currentQty == quantity)
                        {
                            using (SqlCommand deleteCmd = new SqlCommand("DELETE FROM PortfolioItems WHERE UserID = @userId AND StockID = @stockId", conn, transaction))
                            {
                                deleteCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                                deleteCmd.Parameters.Add("@stockId", SqlDbType.Int).Value = stockId;
                                await deleteCmd.ExecuteNonQueryAsync();
                            }
                        }
                        else
                        {
                            using (SqlCommand updateCmd = new SqlCommand("UPDATE PortfolioItems SET Quantity = Quantity - @qty WHERE UserID = @userId AND StockID = @stockId", conn, transaction))
                            {
                                updateCmd.Parameters.Add("@qty", SqlDbType.Decimal).Value = quantity;
                                updateCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                                updateCmd.Parameters.Add("@stockId", SqlDbType.Int).Value = stockId;
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

                        // Enregistrer la transaction
                        using (SqlCommand transCmd = new SqlCommand("INSERT INTO Transactions (UserID, StockID, TransacType, Quantity, TransacDate) VALUES (@userId, @stockId, 'SELL', @qty, @date)", conn, transaction))
                        {
                            transCmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                            transCmd.Parameters.Add("@stockId", SqlDbType.Int).Value = stockId;
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

            // Cleaned up the query string
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
        public async Task<List<(int TransactionID, string StockName, string Type, float Quantity, DateTime Date)>> GetTransactionHistory(int userId)
        {
            var history = new List<(int, string, string, float, DateTime)>();
            string query = @"SELECT t.TransactionID, s.StockName, t.TransacType, t.Quantity, t.TransacDate
                             FROM Transactions t
                             JOIN Stocks s ON t.StockID = s.StockID
                             WHERE t.UserID = @userId
                             ORDER BY t.TransacDate DESC";

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
                                (float)reader["Quantity"],
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
    }
}