using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UAE_Recharge.Core.Models;

namespace UAE_Recharge.Core.Database
{
    public class DatabaseContext
    {
        public SQLiteAsyncConnection Connection { get; }

        public DatabaseContext(string databaseFileName)
        {
            var databaseFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "databases");
            if (!Directory.Exists(databaseFolderPath))
            {
                Directory.CreateDirectory(databaseFolderPath);
            }

            var databasePath = Path.Combine(databaseFolderPath, databaseFileName);

            if (!File.Exists(databasePath))
            {
                // Copy the database file from the Assets folder if it doesn't exist
                CopyDatabaseFromAssets(databasePath, databaseFileName).Wait();
            }

            Connection = new SQLiteAsyncConnection(databasePath);
            InitializeTables();
            SeedData(); // Wait for seeding data to complete
        }

        private async Task CopyDatabaseFromAssets(string databasePath, string databaseFileName)
        {
            using (var assetStream = Android.App.Application.Context.Assets.Open(databaseFileName))
            using (var fileStream = new FileStream(databasePath, FileMode.Create, FileAccess.Write))
            {
                await assetStream.CopyToAsync(fileStream);
            }
        }

        private void InitializeTables()
        {
            Connection.CreateTableAsync<User>().Wait();
            Connection.CreateTableAsync<Transaction>().Wait();
            Connection.CreateTableAsync<Beneficiary>().Wait();
            // Add more tables as needed
        }

        private async Task SeedData()
        {
            try
            {
                // Delete existing users
                await Connection.DeleteAllAsync<User>();

                //// Delete existing beneficiaries
                //await Connection.DeleteAllAsync<Beneficiary>();

                //// Delete existing transactions
                //await Connection.DeleteAllAsync<Transaction>();

                // Check if data already exists
                var existingUsers = await Connection.Table<User>().CountAsync();
                //var existingBeneficiaries = await Connection.Table<Beneficiary>().CountAsync();
                //var existingTransactions = await Connection.Table<Transaction>().CountAsync();

                // If data doesn't exist, insert mock data
                if (existingUsers == 0)
                {
                // Insert mock data for users
                await Connection.InsertAllAsync(new List<User>
                    {
                        new User {Id = 1, Username = "ahmed1", PhoneNumber = "0506901530", Password = "password1", Balance = 3500, IsSynced = true, IsVerified = true },
                        new User {Id = 2, Username = "ahmed2", PhoneNumber = "0987654321", Password = "password2", Balance = 3500, IsSynced = true, IsVerified = false }
                    });

                    //        // Insert mock data for beneficiaries
                    //await Connection.InsertAllAsync(new List<Beneficiary>
                    //{
                    //    new Beneficiary {Id = 1, UserId = 1, Nickname = "friend1", PhoneNumber = "1111111111", TotalTopUps = 50, IsSynced = true },
                    //    new Beneficiary {Id = 2, UserId = 2, Nickname = "friend2", PhoneNumber = "2222222222", TotalTopUps = 100, IsSynced = true }
                    //});

                    //        // Insert mock data for transactions
                    //        await Connection.InsertAllAsync(new List<Transaction>
                    //{
                    //    new Transaction {Id = 1, UserId = 1, BeneficiaryId = 1, Amount = 50, Timestamp = DateTime.Now, IsSynced = true },
                    //    new Transaction {Id = 2, UserId = 2, BeneficiaryId = 2, Amount = 100, Timestamp = DateTime.Now, IsSynced = true }
                    //});

                    // Add more mock data as needed
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine($"Error seeding data: {ex.Message}");
            }
        }

    }
}
