using System;
using NUnit.Framework;
using Xamarin.UITest;

namespace UAE_Charge_UITest
{
    [TestFixture(Platform.Android)]
    public class HomeActivityTests
    {
        IApp app;
        Platform platform;

        public HomeActivityTests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
        }

        [Test]
        public void BeneficiariesListIsDisplayed()
        {
            // Wait for the "Add Beneficiary" button to appear
            app.WaitForElement(c => c.Marked("addBeneficiaryButton"));

            // Tap on the "Add Beneficiary" button
            app.Tap(c => c.Marked("addBeneficiaryButton"));

            // Check if the RecyclerView with beneficiaries is displayed
            Assert.IsTrue(app.Query(c => c.Marked("beneficiariesRecyclerView")).Length > 0);

            // Take a screenshot
            app.Screenshot("Beneficiaries List Screen");
        }

        [Test]
        public void TransactionHistoryButtonIsClickable()
        {
            // Wait for the "Transaction History" button to appear
            app.WaitForElement(c => c.Marked("transactionHistoryButton"));

            // Tap on the "Transaction History" button
            app.Tap(c => c.Marked("transactionHistoryButton"));

            // Check if the app navigated to the Transaction History Activity
            Assert.IsTrue(app.Query(c => c.Marked("Transaction History")).Length > 0);

            // Take a screenshot
            app.Screenshot("Transaction History Screen");
        }
    }
}
