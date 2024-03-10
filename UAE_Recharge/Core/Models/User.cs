using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UAE_Recharge.Core.Models
{
    public class User
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public int Balance { get; set; }
        public bool IsVerified { get; set; } // Indicates if the user has been verfied

        public bool IsSynced { get; set; } // Indicates if the user has been synced
    }
}