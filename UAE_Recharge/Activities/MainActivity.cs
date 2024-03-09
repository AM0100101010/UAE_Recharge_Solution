using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Serilog;
using Serilog.Events;
using System.IO;
using UAE_Recharge.Core.Database;

namespace UAE_Recharge
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // Initialize Serilog logger
            string logFilePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "log.txt");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(logFilePath, LogEventLevel.Debug)
                .CreateLogger();


            SetContentView(Resource.Layout.activity_main);

            // Find the TextView for the app name
            var appNameTextView = FindViewById<TextView>(Resource.Id.appNameTextView);
            appNameTextView.Text = GetString(Resource.String.app_name);

            // Find the TextView for the app description
            var appDescriptionTextView = FindViewById<TextView>(Resource.Id.appDescriptionTextView);
            appDescriptionTextView.Text = GetString(Resource.String.app_description);

            // Find the Button for signing in
            var signInButton = FindViewById<Button>(Resource.Id.signInButton);
            signInButton.Click += SignInButton_Click;

            // Find the Button for signing up
            var signUpButton = FindViewById<Button>(Resource.Id.signUpButton);
            signUpButton.Click += SignUpButton_Click;
        }

        private void SignInButton_Click(object sender, System.EventArgs e)
        {
            // Navigate to sign-in activity
            StartActivity(new Intent(this, typeof(SignInActivity)));
        }

        private void SignUpButton_Click(object sender, System.EventArgs e)
        {
            // Navigate to sign-up activity
            StartActivity(new Intent(this, typeof(SignUpActivity)));
        }

        protected override void OnDestroy()
        {
            // Dispose the Serilog logger when the activity is destroyed
            Log.CloseAndFlush();
            base.OnDestroy();
        }


    }
}
