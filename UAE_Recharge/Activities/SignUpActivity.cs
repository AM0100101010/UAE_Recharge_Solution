using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;
using UAE_Recharge.Core.Database;
using UAE_Recharge.Core.Interfaces;
using UAE_Recharge.Core.Models;
using UAE_Recharge.Core.Services;

namespace UAE_Recharge
{
    [Activity(Label = "Sign Up")]
    public class SignUpActivity : Activity
    {
        private EditText usernameEditText;
        private EditText phoneNumberEditText;
        private EditText passwordEditText;
        private Button signUpButton;
        private IUserService userService;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_sign_up);

            // Initialize UserService with API off and local database context
            var databasePath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "database.db3");
            var dbContext = new DatabaseContext(databasePath);
            userService = new UserService(useApi: false, dbContext: dbContext);

            usernameEditText = FindViewById<EditText>(Resource.Id.usernameEditText);
            phoneNumberEditText = FindViewById<EditText>(Resource.Id.phoneNumberEditText);
            passwordEditText = FindViewById<EditText>(Resource.Id.passwordEditText);
            signUpButton = FindViewById<Button>(Resource.Id.signUpButton);

            signUpButton.Click += SignUpButton_Click;
        }

        private async void SignUpButton_Click(object sender, EventArgs e)
        {
            string username = usernameEditText.Text;
            string phoneNumber = phoneNumberEditText.Text;
            string password = passwordEditText.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(password))
            {
                ShowToast("Please fill all fields");
                return;
            }

            // Validate phone number format and length
            if (!IsValidPhoneNumber(phoneNumber))
            {
                ShowToast("Invalid phone number format or length should be 10 digits starting with 05");
                return;
            }

            // Validate password length
            if (password.Length < 4)
            {
                ShowToast("Password should be at least 4 characters long");
                return;
            }

            // Check if the username is available
            bool isUsernameAvailable = await userService.CheckUsernameAvailabilityAsync(username);
            if (!isUsernameAvailable)
            {
                ShowToast("Username is already taken. Please choose another username.");
                return;
            }

            // Assuming you need to define a default balance for new users
            int balance = 1000;

            var newUser = await userService.CreateUserAsync(username, phoneNumber, password, balance);
            if (newUser != null)
            {
                // Sign-up successful, start main activity and pass user info
                StartMainActivity(newUser);
            }
            else
            {
                ShowToast("Failed to create user. Please try again.");
            }
        }

        private void StartMainActivity(User user)
        {
            Intent intent = new Intent(this, typeof(HomeActivity));
            intent.PutExtra("User", Newtonsoft.Json.JsonConvert.SerializeObject(user));
            StartActivity(intent);
            Finish(); // Finish the current activity to prevent going back to sign-up screen
        }

        private void ShowToast(string message)
        {
            Toast.MakeText(this, message, ToastLength.Short).Show();
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // Validate phone number format (starts with 05) and length (10 digits)
            return phoneNumber.StartsWith("05") && phoneNumber.Length == 10;
        }
    }
}
