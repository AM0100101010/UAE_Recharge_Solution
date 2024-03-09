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
    public interface IUserService
    {
        Task<User> CreateUserAsync(string username, string phoneNumber, string password, int balance);
        Task<bool> VerifyUserAsync(string username, string password);

        //VerifyAndGetUserAsync
        Task<User> VerifyAndGetUserAsync(string username, string password);
        Task<bool> DeductBalanceAsync(int userId, int amount);

        Task<bool> CheckUsernameAvailabilityAsync(string username);
    }
}