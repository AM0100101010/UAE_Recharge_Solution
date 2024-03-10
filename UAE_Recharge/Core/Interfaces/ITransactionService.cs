using System.Collections.Generic;
using System.Threading.Tasks;
using UAE_Recharge.Core.Models;

namespace UAE_Recharge.Core.Interfaces
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetTransactionsAsync(int userId);
        Task<Transaction> CreateTransactionAsync(int userId, int beneficiaryId, string beneficiaryNickname, int amount);
        Task<bool> SyncTransactionsAsync(int userId);
        Task<bool> CanUserTopUpAsync(User user, Beneficiary beneficiary, decimal amount);
        Task<decimal> GetTotalToppedUpThisMonthAsync(int userId);
        Task<decimal> GetTotalToppedUpThisMonthAsync(int userId, int beneficiaryId);
    }
}
