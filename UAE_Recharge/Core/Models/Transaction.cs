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
    public class Transaction
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BeneficiaryId { get; set; }
        public int Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsSynced { get; set; } // Indicates if the transaction has been synced
    }
}