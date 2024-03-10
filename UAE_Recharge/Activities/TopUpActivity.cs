using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Threading.Tasks;
using UAE_Recharge.Core.Database;
using UAE_Recharge.Core.Interfaces;
using UAE_Recharge.Core.Models;
using UAE_Recharge.Core.Services;

namespace UAE_Recharge
{
    [Activity(Label = "TopUpActivity")]
    public class TopUpActivity : Activity
    {
        private IUserService userService;
        private ITransactionService transactionService;
        private IBeneficiaryService beneficiaryService;
        private User user;
        private Beneficiary beneficiary;

        // UI elements
        private TextView userBalanceTextView;
        private TextView remainingUserBalanceTextView;
        private TextView beneficiaryNicknameTextView;
        private TextView beneficiaryPhoneNumberTextView;
        private TextView remainingBeneficiaryBalanceTextView;
        private Spinner topUpAmountSpinner;
        private Button transferButton;
        private const int ChargeAmount = 1;
        private TextView chargeAmountTextView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Initialize();
            BindUIElements();
            LoadRemainingBalances();
            // Set click listener for transferButton
            transferButton.Click += OnTransferButtonClicked;
        }

        private void Initialize()
        {
            SetContentView(Resource.Layout.activity_top_up);

            // Initialize services
            userService = new UserService(useApi: false, dbContext: new DatabaseContext(System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "database.db3")));
            transactionService = new TransactionService(useApi: false, dbContext: new DatabaseContext(System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "database.db3")));
            beneficiaryService = new BeneficiaryService(useApi: false, dbContext: new DatabaseContext(System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "database.db3")));

            // Retrieve beneficiary and user information from Intent
            string beneficiaryDataJson = Intent.GetStringExtra("Beneficiary");
            string userDataJson = Intent.GetStringExtra("User");
            user = JsonConvert.DeserializeObject<User>(userDataJson);
            beneficiary = JsonConvert.DeserializeObject<Beneficiary>(beneficiaryDataJson);

            // Bind top-up amount Spinner
            topUpAmountSpinner = FindViewById<Spinner>(Resource.Id.topUpAmountSpinner);
            string[] topUpOptions = { "AED 5", "AED 10", "AED 20", "AED 30", "AED 50", "AED 75", "AED 100" };
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, topUpOptions);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            topUpAmountSpinner.Adapter = adapter;
        }

        private void BindUIElements()
        {
            // Bind UI elements
            userBalanceTextView = FindViewById<TextView>(Resource.Id.userBalanceTextView);
            remainingUserBalanceTextView = FindViewById<TextView>(Resource.Id.remainingUserBalanceTextView);
            beneficiaryNicknameTextView = FindViewById<TextView>(Resource.Id.beneficiaryNicknameTextView);
            beneficiaryPhoneNumberTextView = FindViewById<TextView>(Resource.Id.beneficiaryPhoneNumberTextView);
            remainingBeneficiaryBalanceTextView = FindViewById<TextView>(Resource.Id.remainingBeneficiaryBalanceTextView);
            transferButton = FindViewById<Button>(Resource.Id.transferButton);
            chargeAmountTextView = FindViewById<TextView>(Resource.Id.chargeAmountTextView); 
        }

        private void LoadRemainingBalances()
        {
            // Calculate remaining balances
            //decimal totalToppedUpThisMonth = transactionService.GetTotalToppedUpThisMonthAsync(user.Id).Result;
            //decimal remainingUserBalance = 3000 - totalToppedUpThisMonth;
            //decimal remainingBeneficiaryBalance = user.IsVerified ? 500 - transactionService.GetTotalToppedUpThisMonthAsync(user.Id, beneficiary.Id).Result : 1000 - transactionService.GetTotalToppedUpThisMonthAsync(user.Id, beneficiary.Id).Result;

            // Determine the remaining beneficiary balance based on user verification status
            string remainingBalanceText;
            if (user.IsVerified)
            {
                remainingBalanceText = $"Monthly limit for {beneficiary.Nickname}: AED 500";
            }
            else
            {
                remainingBalanceText = $"Monthly limit for {beneficiary.Nickname}: AED 1000";
            }

            // Bind values to UI elements
            RunOnUiThread(() =>
            {
                userBalanceTextView.Text = user.Balance.ToString();
                //remainingUserBalanceTextView.Text = $"Remaining for user: AED {remainingUserBalance}";
                beneficiaryNicknameTextView.Text = beneficiary.Nickname;
                beneficiaryPhoneNumberTextView.Text = beneficiary.PhoneNumber;
                remainingBeneficiaryBalanceTextView.Text = remainingBalanceText;// $"Remaining for {beneficiary.Nickname}: AED {remainingBeneficiaryBalance}";
            });
        }


        private async void OnTransferButtonClicked(object sender, EventArgs e)
        {
            // Get the selected top-up amount from the Spinner
            string selectedAmountText = topUpAmountSpinner.SelectedItem.ToString();
            int amount = int.Parse(selectedAmountText.Replace("AED ", ""));
            amount = amount + ChargeAmount;
            try
            {
                // Check if the amount exceeds the user's remaining balance
                if (amount > user.Balance)
                {
                    Toast.MakeText(this, "Insufficient balance", ToastLength.Short).Show();
                    return;
                }

                // Check if the amount exceeds the monthly limit for the user
                decimal totalToppedUpThisMonth = await transactionService.GetTotalToppedUpThisMonthAsync(user.Id);
                decimal remainingUserBalance = 3000 - totalToppedUpThisMonth;

                if (amount > remainingUserBalance)
                {
                    Toast.MakeText(this, "Monthly limit exceeded", ToastLength.Short).Show();
                    return;
                }

                // Check if the user can top up the beneficiary
                if (!await transactionService.CanUserTopUpAsync(user, beneficiary, amount))
                {
                    Toast.MakeText(this, "Monthly limit exceeded for this beneficiary", ToastLength.Short).Show();
                    return;
                }

                // Deduct the amount from the user's balance
                bool isBalanceDeducted = await userService.DeductBalanceAsync(user.Id, amount);
                if (!isBalanceDeducted)
                {
                    Toast.MakeText(this, "Failed to deduct balance", ToastLength.Short).Show();
                    return;
                }

                // Create the transaction
                await transactionService.CreateTransactionAsync(user.Id, beneficiary.Id, beneficiary.Nickname, amount);

                // Update the total top-ups for the beneficiary
                bool isTopUpsUpdated = await beneficiaryService.UpdateTotalTopUpsAsync(beneficiary.Id, amount);
                if (!isTopUpsUpdated)
                {
                    Log.Error("Failed to update total top-ups for the beneficiary.");
                    return;
                }

                // Update the user's balance locally (if needed)
                user.Balance -= amount;

                // Display a success message
                Toast.MakeText(this, $"Successfully topped up {beneficiary.Nickname} with {amount - ChargeAmount} AED (including {ChargeAmount} AED charge)", ToastLength.Short).Show();

                // Refresh the HomeActivity by recreating it
                var homeActivityIntent = new Intent(this, typeof(HomeActivity));
                homeActivityIntent.PutExtra("User", JsonConvert.SerializeObject(user));
                StartActivity(homeActivityIntent);

                Finish(); // Close the activity
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating transaction: {Message}", ex.Message);
                Toast.MakeText(this, "An error occurred while processing the transaction", ToastLength.Short).Show();
            }
        }
    }
}
