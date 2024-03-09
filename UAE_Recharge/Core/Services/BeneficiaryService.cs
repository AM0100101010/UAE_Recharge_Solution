using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UAE_Recharge.Core.Api;
using UAE_Recharge.Core.Database;
using UAE_Recharge.Core.Interfaces;
using UAE_Recharge.Core.Models;

namespace UAE_Recharge.Core.Services
{
    public class BeneficiaryService : IBeneficiaryService
    {
        private readonly ApiClient _apiClient; // ApiClient instance for API communication
        private readonly DatabaseContext _dbContext; // DatabaseContext instance for local database access
        private readonly bool _useApi; // Flag indicating whether to use API or local database

        public BeneficiaryService(bool useApi, ApiClient apiClient = null, DatabaseContext dbContext = null)
        {
            _useApi = useApi; // Set the useApi flag

            // Initialize ApiClient if useApi is true and apiClient is provided
            if (_useApi)
            {
                _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            }
            // Initialize DatabaseContext if useApi is false and dbContext is provided
            else
            {
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            }
        }

        public async Task<List<Beneficiary>> GetBeneficiariesAsync(int userId)
        {
            if (_useApi)
            {
                // Call the API to get beneficiaries for the given user ID
                // Assuming you have an endpoint to retrieve beneficiaries
                // Example:
                return await _apiClient.GetAsync<List<Beneficiary>>($"beneficiaries?userId={userId}");
            }
            else
            {
                // Retrieve beneficiaries from the local SQLite database
                return await _dbContext.Connection.Table<Beneficiary>()
                                                  .Where(b => b.UserId == userId)
                                                  .ToListAsync();
            }
        }

        public async Task<Beneficiary> CreateBeneficiaryAsync(int userId, string nickname, string phoneNumber)
        {
            int newId;

            // Retrieve all beneficiaries
            var allBeneficiaries = await _dbContext.Connection.Table<Beneficiary>().ToListAsync();


            newId = allBeneficiaries.Count() + 1;


            var beneficiary = new Beneficiary
            {
                Id = newId,
                UserId = userId,
                Nickname = nickname,
                PhoneNumber = phoneNumber,
                TotalTopUps = 0,
                IsSynced = false
            };

            if (_useApi)
            {
                // Call the API to create the beneficiary
                return await _apiClient.PostAsync<Beneficiary>("beneficiaries", beneficiary);
            }
            else
            {
                // Insert the beneficiary into the local SQLite database
                await _dbContext.Connection.InsertAsync(beneficiary);
                return beneficiary;
            }
        }

        public async Task<bool> SyncBeneficiariesAsync(int userId)
        {
            if (_useApi)
            {
                // Get beneficiaries from local SQLite database that are not synced
                var beneficiaries = await _dbContext.Connection.Table<Beneficiary>()
                                                               .Where(b => b.UserId == userId && !b.IsSynced)
                                                               .ToListAsync();

                // Sync beneficiaries to the API
                foreach (var beneficiary in beneficiaries)
                {
                    // Call the API to sync the beneficiary
                    // Assuming you have an endpoint to sync beneficiaries
                    // Example:
                    await _apiClient.PostAsync<Beneficiary>("beneficiaries/sync", beneficiary);

                    // Update the IsSynced flag
                    beneficiary.IsSynced = true;
                    await _dbContext.Connection.UpdateAsync(beneficiary);
                }

                return true; // Return true for successful sync
            }
            else
            {
                // Implement logic to sync beneficiaries with the local SQLite database
                // For example, you might download new beneficiaries from the API and store them locally
                // Assuming you have an endpoint to retrieve new beneficiaries
                // Example:
                var newBeneficiaries = await _apiClient.GetAsync<List<Beneficiary>>($"beneficiaries?userId={userId}");

                // Insert new beneficiaries into the local SQLite database
                foreach (var newBeneficiary in newBeneficiaries)
                {
                    await _dbContext.Connection.InsertAsync(newBeneficiary);
                }

                return true; // Return true for successful sync
            }
        }

        public async Task<bool> UpdateTotalTopUpsAsync(int beneficiaryId, int additionalTopUps)
        {
            try
            {
                if (_useApi)
                {
                    // Call the API to update the TotalTopUps property
                    // Assuming you have an endpoint to update TotalTopUps
                    // Example:
                    var response = await _apiClient.PostAsync<bool>("beneficiaries/updateTotalTopUps", new { BeneficiaryId = beneficiaryId, AdditionalTopUps = additionalTopUps });
                    return response;
                }
                else
                {
                    // Retrieve all beneficiaries from the local SQLite database
                    var allBeneficiaries = await _dbContext.Connection.Table<Beneficiary>().ToListAsync();

                    // Find the beneficiary with the specified ID
                    var beneficiary = allBeneficiaries.FirstOrDefault(b => b.Id == beneficiaryId);

                    if (beneficiary != null)
                    {
                        // Update the TotalTopUps property
                        beneficiary.TotalTopUps += additionalTopUps;
                        await _dbContext.Connection.UpdateAsync(beneficiary);
                        return true;
                    }
                    else
                    {
                        return false; // Beneficiary not found
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
                return false;
            }
        }

        public async Task<int> GetBeneficiaryCountAsync(int userId)
        {
            if (_useApi)
            {
                // Call the API to get the count of beneficiaries for the user
                // Assuming you have an endpoint to retrieve beneficiary count
                // Example:
                var count = await _apiClient.GetAsync<int>($"beneficiaries/count?userId={userId}");
                return count;
            }
            else
            {
                // Retrieve the count of beneficiaries from the local SQLite database
                var count = await _dbContext.Connection.Table<Beneficiary>()
                                                        .Where(b => b.UserId == userId)
                                                        .CountAsync();
                return count;
            }
        }
    }
}
