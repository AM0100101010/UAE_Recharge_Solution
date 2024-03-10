using NUnit.Framework;
using UAE_Recharge.Core.Services;
using System.Threading.Tasks;
using UAE_Recharge.Core.Models;
using Moq;
using System.Collections.Generic;
using UAE_Recharge.Core.Api;
using UAE_Recharge.Core.Database;

namespace UAE_Recharge.UnitTests
{
    public class TransactionServiceTests
    {
        [Test]
        public async Task GetTransactionsAsync_ReturnsListOfTransactions()
        {
            // Arrange
            var mockApiClient = new Mock<ApiClient>();
            var mockDbContext = new Mock<DatabaseContext>();
            var transactionService = new TransactionService(useApi: true, apiClient: mockApiClient.Object, dbContext: mockDbContext.Object);
            int userId = 1;
            var expectedTransactions = new List<Transaction>();

            mockApiClient.Setup(api => api.GetAsync<List<Transaction>>(It.IsAny<string>()))
                .ReturnsAsync(expectedTransactions);

            // Act
            var transactions = await transactionService.GetTransactionsAsync(userId);

            // Assert
            Assert.IsNotNull(transactions);
            Assert.AreEqual(expectedTransactions, transactions);
        }

        [Test]
        public async Task CreateTransactionAsync_ReturnsCreatedTransaction()
        {
            // Arrange
            var mockDbContext = new Mock<DatabaseContext>();
            var transactionService = new TransactionService(useApi: false, dbContext: mockDbContext.Object);
            int userId = 1;
            int beneficiaryId = 2;
            int amount = 100;
            var expectedTransaction = new Transaction { UserId = userId, BeneficiaryId = beneficiaryId, Amount = amount };

            mockDbContext.Setup(db => db.Connection.InsertAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(1);

            // Act
            var transaction = await transactionService.CreateTransactionAsync(userId, beneficiaryId, amount);

            // Assert
            Assert.(transaction);
            Assert.AreEqual(expectedTransaction.UserId, transaction.UserId);
            Assert.AreEqual(expectedTransaction.BeneficiaryId, transaction.BeneficiaryId);
            Assert.AreEqual(expectedTransaction.Amount, transaction.Amount);
        }
    }
}
