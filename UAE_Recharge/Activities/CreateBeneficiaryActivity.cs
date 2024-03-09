using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UAE_Recharge.Core.Database;
using UAE_Recharge.Core.Interfaces;
using UAE_Recharge.Core.Models;
using UAE_Recharge.Core.Services;

namespace UAE_Recharge
{
    [Activity(Label = "CreateBeneficiaryActivity")]
    public class CreateBeneficiaryActivity : Activity
    {
        private IBeneficiaryService _beneficiaryService;
        private TextView _currentBeneficiaryCountTextView;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_create_beneficiary);

            // Retrieve user data passed from HomeActivity
            string userDataJson = Intent.GetStringExtra("User");
            User user = JsonConvert.DeserializeObject<User>(userDataJson);

            // Initialize UserService with API off and local database context
            var databasePath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "database.db3");
            var dbContext = new DatabaseContext(databasePath);

            // Initialize the beneficiary service
            _beneficiaryService = new BeneficiaryService(useApi: false, dbContext: dbContext); // Assuming you don't want to use the API for beneficiary service

            EditText nicknameEditText = FindViewById<EditText>(Resource.Id.nicknameEditText);
            EditText phoneNumberEditText = FindViewById<EditText>(Resource.Id.phoneNumberEditText);
            Button saveButton = FindViewById<Button>(Resource.Id.saveButton);
            _currentBeneficiaryCountTextView = FindViewById<TextView>(Resource.Id.currentBeneficiaryCountTextView);

            // Get the current count of beneficiaries and update the TextView
            int currentCount = await _beneficiaryService.GetBeneficiaryCountAsync(user.Id);
            // Increment the count by 1 to reflect the addition of the new beneficiary
            currentCount++;
            // Update the beneficiary count TextView
            if (currentCount > 5)
            {
                _currentBeneficiaryCountTextView.Text = "You've reached the maximum limit 5 of 5.";
            }
            else
            {
                _currentBeneficiaryCountTextView.Text = $"{currentCount} of 5";
            }

            saveButton.Click += async (sender, e) =>
            {
                string nickname = nicknameEditText.Text.Trim();
                string phoneNumber = phoneNumberEditText.Text.Trim();

                if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(phoneNumber))
                {
                    ShowToast("Please enter both nickname and phone number");
                    return;
                }

                // Validate phone number format and length
                if (!IsValidPhoneNumber(phoneNumber))
                {
                    ShowToast("Invalid phone number format or length should be 10 digits starting with 05");
                    return;
                }

                // Get the count of beneficiaries for the user
                int userBeneficiaryCount = await _beneficiaryService.GetBeneficiaryCountAsync(user.Id);

                // Check if the user has reached the maximum limit of beneficiaries
                if (userBeneficiaryCount >= 5)
                {
                    ShowToast("You have reached the maximum limit of beneficiaries.");
                    return;
                }

                // Create the beneficiary
                var result = await _beneficiaryService.CreateBeneficiaryAsync(user.Id, nickname, phoneNumber);

                if (result != null)
                {
                    ShowToast("Beneficiary created successfully");

                    // Refresh the HomeActivity by recreating it
                    var homeActivityIntent = new Intent(this, typeof(HomeActivity));
                    homeActivityIntent.PutExtra("User", JsonConvert.SerializeObject(user));
                    StartActivity(homeActivityIntent);

                    Finish(); // Close the activity
                }
                else
                {
                    ShowToast("Failed to create beneficiary");
                }
            };
        }

        private void ShowToast(string message)
        {
            Toast.MakeText(this, message, ToastLength.Short).Show();
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // Validate phone number format (starts with 05) and length (10 digits)
            return Regex.IsMatch(phoneNumber, @"^05\d{8}$");
        }
    }
}
