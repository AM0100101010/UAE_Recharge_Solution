using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using UAE_Recharge.Core.Interfaces;
using UAE_Recharge.Core.Models;
using UAE_Recharge.Core.Services;
using UAE_Recharge.Adapters;
using AndroidX.RecyclerView.Widget;
using UAE_Recharge.Core.Database;

namespace UAE_Recharge
{
    [Activity(Label = "Home")]
    public class HomeActivity : Activity
    {
        private RecyclerView beneficiariesRecyclerView;
        private BeneficiaryAdapter beneficiaryAdapter;
        private List<Beneficiary> beneficiaries;
        private IBeneficiaryService beneficiaryService;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_home);

            // Initialize UserService with API off and local database context
            var databasePath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "database.db3");
            var dbContext = new DatabaseContext(databasePath);


            // Initialize services
            beneficiaryService = new BeneficiaryService(useApi: false, dbContext: dbContext); // don't Use API for beneficiary service

            // Retrieve user data passed from SignInActivity
            string userDataJson = Intent.GetStringExtra("User");
            User user = JsonConvert.DeserializeObject<User>(userDataJson);

            // Set user information in the layout
            SetUserInformation(user);

            // Initialize RecyclerView and Adapter
            beneficiariesRecyclerView = FindViewById<RecyclerView>(Resource.Id.beneficiariesRecyclerView);
            beneficiaries = new List<Beneficiary>();
            beneficiaryAdapter = new BeneficiaryAdapter(this, beneficiaries); // Pass 'this' as the Context

            // Set layout manager and adapter for RecyclerView
            beneficiariesRecyclerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false));
            beneficiariesRecyclerView.SetAdapter(beneficiaryAdapter);

            // Load beneficiaries
            LoadBeneficiaries(user.Id);

            // Find the "Add Beneficiary" button
            Button addBeneficiaryButton = FindViewById<Button>(Resource.Id.addBeneficiaryButton);

            // Add click listener to the "Add Beneficiary" button
            addBeneficiaryButton.Click += (sender, e) =>
            {
                // Launch CreateBeneficiaryActivity passing the user data
                Intent intent = new Intent(this, typeof(CreateBeneficiaryActivity));
                intent.PutExtra("User", JsonConvert.SerializeObject(user));
                StartActivity(intent);
            };
        }

        private async void LoadBeneficiaries(int userId)
        {
            // Retrieve beneficiaries from the service
            beneficiaries = await beneficiaryService.GetBeneficiariesAsync(userId);

            // Update the adapter with the new list of beneficiaries
            beneficiaryAdapter.UpdateData(beneficiaries);
        }

        private void SetUserInformation(User user)
        {
            // Find TextViews in the layout
            TextView usernameTextView = FindViewById<TextView>(Resource.Id.usernameTextView);
            TextView phoneNumberTextView = FindViewById<TextView>(Resource.Id.phoneNumberTextView);
            TextView balanceTextView = FindViewById<TextView>(Resource.Id.balanceTextView);

            // Set user information to TextViews
            usernameTextView.Text = user.Username;
            phoneNumberTextView.Text = user.PhoneNumber;
            balanceTextView.Text = user.Balance.ToString(); // Convert balance to string before setting
        }

        // Method to refresh the list of beneficiaries
        public async void RefreshBeneficiaries(int userId)
        {
            // Reload beneficiaries
            LoadBeneficiaries(userId);
        }
    }
}
