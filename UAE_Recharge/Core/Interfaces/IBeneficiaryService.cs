using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAE_Recharge.Core.Models;

namespace UAE_Recharge.Core.Interfaces
{
    public interface IBeneficiaryService
    {
        Task<List<Beneficiary>> GetBeneficiariesAsync(int userId);
        Task<Beneficiary> CreateBeneficiaryAsync(int userId, string nickname, string phoneNumber);
        Task<bool> SyncBeneficiariesAsync(int userId);
        Task<bool> UpdateTotalTopUpsAsync(int beneficiaryId, int additionalTopUps);
        Task<int> GetBeneficiaryCountAsync(int userId);
    }
}