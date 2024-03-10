using Android.App;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using UAE_Recharge.Core.Interfaces;
using UAE_Recharge.Core.Models;
using UAE_Recharge.Adapters;
using AndroidX.RecyclerView.Widget;
using UAE_Recharge.Core.Database;
using UAE_Recharge.Core.Services;

namespace UAE_Recharge
{
    [Activity(Label = "Transaction History")]
    public class TransactionHistoryActivity : Activity
    {
        private RecyclerView transactionRecyclerView;
        private TransactionAdapter transactionAdapter;
        private List<Transaction> transactions;
        private ITransactionService transactionService;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_transaction_history);

            // Initialize TransactionService with API off and local database context
            var databasePath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "database.db3");
            var dbContext = new DatabaseContext(databasePath);
            transactionService = new TransactionService(useApi: false, dbContext: dbContext);

            // Retrieve user data passed from HomeActivity
            string userDataJson = Intent.GetStringExtra("User");
            User user = JsonConvert.DeserializeObject<User>(userDataJson);

            // Initialize RecyclerView and Adapter
            transactionRecyclerView = FindViewById<RecyclerView>(Resource.Id.transactionRecyclerView);
            transactions = new List<Transaction>();
            transactionAdapter = new TransactionAdapter(this, transactions);

            // Set layout manager and adapter for RecyclerView
            transactionRecyclerView.SetLayoutManager(new LinearLayoutManager(this));
            transactionRecyclerView.SetAdapter(transactionAdapter);

            // Load transactions
            LoadTransactions(user.Id);
        }

        private async void LoadTransactions(int userId)
        {
            // Retrieve transactions from the service
            transactions = await transactionService.GetTransactionsAsync(userId);

            // Update the adapter with the new list of transactions
            transactionAdapter.UpdateData(transactions);
        }
    }
}
