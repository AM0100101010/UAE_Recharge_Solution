using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using UAE_Recharge.Core.Api;
using UAE_Recharge.Core.Database;
using UAE_Recharge.Core.Interfaces;
using UAE_Recharge.Core.Models;

namespace UAE_Recharge.Core.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApiClient _apiClient;
        private readonly DatabaseContext _dbContext;
        private readonly bool _useApi;
        private readonly ILogger _logger;

        public TransactionService(bool useApi, ApiClient apiClient = null, DatabaseContext dbContext = null)
        {
            _useApi = useApi;
            if (_useApi)
            {
                _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            }
            else
            {
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            }

            _logger = Log.ForContext<TransactionService>();
        }

        public async Task<List<Transaction>> GetTransactionsAsync(int userId)
        {
            try
            {
                if (_useApi)
                {
                    // Call the API to get transactions for the given user ID
                    // Assuming you have an endpoint to retrieve transactions
                    // Example:
                    return await _apiClient.GetAsync<List<Transaction>>($"transactions?userId={userId}");
                }
                else
                {
                    // Retrieve transactions from the local SQLite database
                    return await _dbContext.Connection.Table<Transaction>()
                                                      .Where(t => t.UserId == userId)
                                                      .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while getting transactions.");
                throw;
            }
        }


        public async Task<Transaction> CreateTransactionAsync(int userId, int beneficiaryId, string beneficiaryNickname, int amount)
        {
            if (_dbContext == null)
            {
                throw new ArgumentNullException(nameof(_dbContext), "DatabaseContext is not provided.");
            }

            int newId;

            // Retrieve all transactions
            var allTransactions = await _dbContext.Connection.Table<Transaction>().ToListAsync();

            newId = allTransactions.Count() + 1; 
            var transaction = new Transaction
            {
                Id = newId,
                UserId = userId,
                BeneficiaryId = beneficiaryId,
                Amount = amount,
                Timestamp = DateTime.UtcNow,
                IsSynced = false,
                BeneficiaryNickname = beneficiaryNickname
            };

            try
            {
                if (_useApi)
                {
                    // Call the API to create the transaction
                    return await _apiClient.PostAsync<Transaction>("transactions", transaction);
                }
                else
                {
                    // Insert the transaction into the local SQLite database
                    await _dbContext.Connection.InsertAsync(transaction);
                    return transaction;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while creating transaction.");
                throw;
            }
        }

        public async Task<bool> SyncTransactionsAsync(int userId)
        {
            try
            {
                if (_useApi)
                {
                    // Get transactions from local SQLite database that are not synced
                    var transactions = await _dbContext.Connection.Table<Transaction>()
                                                                   .Where(t => t.UserId == userId && !t.IsSynced)
                                                                   .ToListAsync();

                    // Sync transactions to the API
                    foreach (var transaction in transactions)
                    {
                        // Call the API to sync the transaction
                        // Assuming you have an endpoint to sync transactions
                        // Example:
                        await _apiClient.PostAsync<Transaction>("transactions/sync", transaction);

                        // Update the IsSynced flag
                        transaction.IsSynced = true;
                        await _dbContext.Connection.UpdateAsync(transaction);
                    }

                    return true;
                }
                else
                {
                    // Implement logic to sync transactions with the local SQLite database
                    // For example, you might upload unsynced transactions to the API
                    // and update the IsSynced flag accordingly
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while syncing transactions.");
                throw;
            }
        }

        public async Task<bool> CanUserTopUpAsync(User user, Beneficiary beneficiary, decimal amount)
        {
            // Check if the user is verified
            bool isVerified = user.IsVerified;

            // Get the current month and year
            int currentMonth = DateTime.UtcNow.Month;
            int currentYear = DateTime.UtcNow.Year;

            // Filter transactions for the current month and year
            var transactions = await GetTransactionsForMonthAsync(user.Id, currentMonth, currentYear);

            // Calculate the total amount topped up for the current month
            decimal totalAmountToppedUp = transactions.Sum(t => t.Amount);

            // Check if the user can top up the beneficiary within the monthly limits
            if (!isVerified && totalAmountToppedUp + amount <= 1000)
            {
                // User is not verified and can top up within the limit of AED 1,000 per beneficiary per month
                return true;
            }
            else if (isVerified && totalAmountToppedUp + amount <= 500)
            {
                // User is verified and can top up within the limit of AED 500 per beneficiary per month
                return true;
            }
            else
            {
                // User has exceeded the monthly limit
                return false;
            }
        }


        private async Task<List<Transaction>> GetTransactionsForMonthAsync(int userId, int month, int year)
        {
            try
            {
                // Retrieve all transactions for the specified user
                var allTransactions = await _dbContext.Connection.Table<Transaction>()
                                                      .Where(t => t.UserId == userId)
                                                      .ToListAsync();

                // Filter transactions for the specified month and year
                var transactionsForMonth = allTransactions.Where(t => t.Timestamp.Month == month && t.Timestamp.Year == year).ToList();

                return transactionsForMonth;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while getting transactions for the month.");
                throw;
            }
        }

        public async Task<decimal> GetTotalToppedUpThisMonthAsync(int userId)
        {
            var transactions = await GetTransactionsForMonthAsync(userId, DateTime.UtcNow.Month, DateTime.UtcNow.Year);
            return transactions.Sum(t => t.Amount);
        }

        public async Task<decimal> GetTotalToppedUpThisMonthAsync(int userId, int beneficiaryId)
        {
            var transactions = await GetTransactionsForMonthAsync(userId, DateTime.UtcNow.Month, DateTime.UtcNow.Year);
            return transactions.Where(t => t.BeneficiaryId == beneficiaryId).Sum(t => t.Amount);
        }

    }
}
