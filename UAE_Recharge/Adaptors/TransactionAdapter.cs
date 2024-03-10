using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System.Collections.Generic;
using UAE_Recharge.Core.Models;

namespace UAE_Recharge.Adapters
{
    public class TransactionAdapter : RecyclerView.Adapter
    {
        private readonly Context _context;
        private List<Transaction> _transactions;

        public TransactionAdapter(Context context, List<Transaction> transactions)
        {
            _context = context;
            _transactions = transactions;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the item layout
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.transaction_item, parent, false);

            // Create and return the ViewHolder
            return new TransactionViewHolder(itemView);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            // Bind data to ViewHolder
            if (holder is TransactionViewHolder transactionViewHolder)
            {
                Transaction transaction = _transactions[position];
                transactionViewHolder.Bind(transaction);
            }
        }

        public override int ItemCount => _transactions.Count;

        public void UpdateData(List<Transaction> transactions)
        {
            _transactions = transactions;
            NotifyDataSetChanged();
        }
    }

    public class TransactionViewHolder : RecyclerView.ViewHolder
    {
        private TextView _beneficiaryTextView;
        private TextView _amountTextView;
        private TextView _timestampTextView;

        public TransactionViewHolder(View itemView) : base(itemView)
        {
            // Find and initialize views
            _beneficiaryTextView = itemView.FindViewById<TextView>(Resource.Id.beneficiaryTextView);
            _amountTextView = itemView.FindViewById<TextView>(Resource.Id.amountTextView);
            _timestampTextView = itemView.FindViewById<TextView>(Resource.Id.timestampTextView);
        }

        public void Bind(Transaction transaction)
        {
            // Bind data to views
            _beneficiaryTextView.Text = transaction.BeneficiaryNickname;
            _amountTextView.Text = $"Amount: {transaction.Amount} AED";
            _timestampTextView.Text = $"Date: {transaction.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")}";
        }
    }
}
