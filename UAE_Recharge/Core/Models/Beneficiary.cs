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
    public class Beneficiary
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Nickname { get; set; }
        public string PhoneNumber { get; set; }
        public int TotalTopUps { get; set; }
        public bool IsSynced { get; set; } // Indicates if the beneficiary has been synced

        //public string Month { get; set; }

        //public int Amount_Allowed { get; set; }
    }
}