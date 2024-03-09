using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using UAE_Recharge.Core.Api;
using UAE_Recharge.Core.Database;
using UAE_Recharge.Core.Interfaces;
using UAE_Recharge.Core.Models;

namespace UAE_Recharge.Core.Services
{
    public class UserService : IUserService
    {
        private readonly ApiClient _apiClient;
        private readonly DatabaseContext _dbContext;
        private readonly bool _useApi;
        private readonly ILogger _logger = Log.ForContext<UserService>();

        public UserService(bool useApi, ApiClient apiClient = null, DatabaseContext dbContext = null)
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
        }

        public async Task<User> CreateUserAsync(string username, string phoneNumber, string password, int balance)
        {
            _logger.Information("Creating user: {Username}, Phone Number: {PhoneNumber}, Balance: {Balance}", username, phoneNumber, balance);

            if (_useApi)
            {
                // Call the API to create the user
                var user = new User
                {
                    Username = username,
                    PhoneNumber = phoneNumber,
                    Password = password,
                    Balance = balance,
                    IsSynced = false
                };

                // Here you would have an endpoint in your API to handle user creation
                return await _apiClient.PostAsync<User>("users", user);
            }
            else
            {
                int newId;

                // Retrieve all users
                var allUsers = await _dbContext.Connection.Table<User>().ToListAsync();

                newId = allUsers.Count() + 1; 
                // Insert the user into the local SQLite database
                var user = new User
                {
                    Id = newId,
                    Username = username,
                    PhoneNumber = phoneNumber,
                    Password = password,
                    Balance = balance,
                    IsSynced = false,
                    IsVerified=false
                };

                await _dbContext.Connection.InsertAsync(user);

                return user;
            }
        }

        public async Task<User> VerifyAndGetUserAsync(string username, string password)
        {
            // Verify the user
            var isVerified = await VerifyUserAsync(username, password);
            if (!isVerified)
            {
                return null; // User verification failed
            }

            // Retrieve the user from API or local database based on the flag
            if (_useApi)
            {
                // Call the API to get the user
                // Assuming you have an endpoint to retrieve user details
                // Example:
                var user = await _apiClient.GetAsync<User>($"users/{username}");
                return user;
            }
            else
            {
                // Retrieve the user from the local SQLite database
                var user = await _dbContext.Connection.Table<User>()
                                                    .Where(u => u.Username == username && u.Password == password)
                                                    .FirstOrDefaultAsync();

                return user;
            }
        }

        public async Task<bool> VerifyUserAsync(string username, string password)
        {
            _logger.Information("Verifying user: {Username}", username);

            if (_useApi)
            {
                // Call the API to verify the user
                // Assuming you have an endpoint to verify user credentials
                // Example:
                var response = await _apiClient.PostAsync<bool>("users/verify", new { Username = username, Password = password });
                return response;
            }
            else
            {
                // Retrieve the user from the local SQLite database
                var user = await _dbContext.Connection.Table<User>()
                                                    .Where(u => u.Username == username && u.Password == password)
                                                    .FirstOrDefaultAsync();

                return user != null;
            }
        }

        public async Task<bool> DeductBalanceAsync(int userId, int amount)
        {
            try
            {
                if (_useApi)
                {
                    // Call the API to deduct the balance
                    // Assuming you have an endpoint to deduct balance
                    // Example:
                    var response = await _apiClient.PostAsync<bool>("users/deductBalance", new { UserId = userId, Amount = amount });
                    return response;
                }
                else
                {
                    // Retrieve the user from the local SQLite database
                    var user = await _dbContext.Connection.Table<User>()
                                                        .Where(u => u.Id == userId)
                                                        .FirstOrDefaultAsync();

                    if (user != null && user.Balance >= amount)
                    {
                        // Deduct the balance
                        user.Balance -= amount;
                        await _dbContext.Connection.UpdateAsync(user);
                        return true;
                    }
                    else
                    {
                        return false; // Insufficient balance or user not found
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while deducting balance.");
                return false;
            }
        }

        public async Task<bool> CheckUsernameAvailabilityAsync(string username)
        {
            if (_useApi)
            {
                // Call the API to check if the username is available
                // Assuming you have an endpoint to check username availability
                // Example:
                var response = await _apiClient.PostAsync<bool>("users/checkUsernameAvailability", new { Username = username });
                return response;
            }
            else
            {
                // Check if the username exists in the local SQLite database
                var existingUser = await _dbContext.Connection.Table<User>()
                                                              .Where(u => u.Username == username)
                                                              .FirstOrDefaultAsync();
                return existingUser == null; // If existingUser is null, the username is available
            }

        }
    }
}
