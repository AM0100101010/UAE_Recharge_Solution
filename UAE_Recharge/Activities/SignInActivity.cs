using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;
using System.Threading.Tasks;
using UAE_Recharge.Core.Database;
using UAE_Recharge.Core.Models;
using UAE_Recharge.Core.Services;
using UAE_Recharge.Core.Interfaces; // Import the IUserService interface

namespace UAE_Recharge
{
    [Activity(Label = "Sign In")]
    public class SignInActivity : Activity
    {
        private EditText usernameEditText;
        private EditText passwordEditText;
        private Button signInButton;
        private Button signUpButton;

        private IUserService userService; // Change the type to IUserService

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_sign_in);

            // Initialize UserService with API off and local database context
            var databasePath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "database.db3");
            var dbContext = new DatabaseContext(databasePath);
            userService = new UserService(useApi: false, dbContext: dbContext); // Use IUserService instead of UserService

            usernameEditText = FindViewById<EditText>(Resource.Id.usernameEditText);
            passwordEditText = FindViewById<EditText>(Resource.Id.passwordEditText);
            signInButton = FindViewById<Button>(Resource.Id.signInButton);
            signUpButton = FindViewById<Button>(Resource.Id.signUpButton);

            signInButton.Click += SignInButton_Click;
            signUpButton.Click += SignUpTextView_Click; // Set OnClickListener for sign-up TextView
        }

        public async void SignInButton_Click(object sender, EventArgs e)
        {
            string username = usernameEditText.Text;
            string password = passwordEditText.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Toast.MakeText(this, "Please enter username and password", ToastLength.Short).Show();
                return;
            }

            var user = await userService.VerifyAndGetUserAsync(username, password);

            if (user != null)
            {
                // Sign-in successful, start main activity and pass user info
                StartMainActivity(user);
            }
            else
            {
                Toast.MakeText(this, "Invalid username or password", ToastLength.Short).Show();
            }
        }

        private void StartMainActivity(User user)
        {
            Intent intent = new Intent(this, typeof(HomeActivity));
            intent.PutExtra("User", Newtonsoft.Json.JsonConvert.SerializeObject(user));
            StartActivity(intent);
            Finish(); // Finish the current activity to prevent going back to sign-in screen
        }

        public void SignUpTextView_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SignUpActivity));
            StartActivity(intent);
        }
    }
}
