using System;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Android.Content;
using UAE_Recharge.Core.Models;
using Newtonsoft.Json;
using Android.App;

namespace UAE_Recharge.Adapters
{
    public class BeneficiaryAdapter : RecyclerView.Adapter
    {
        private List<Beneficiary> beneficiaries;
        private Context context;

        public BeneficiaryAdapter(Context context, List<Beneficiary> beneficiaries)
        {
            this.context = context;
            this.beneficiaries = beneficiaries;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_beneficiary, parent, false);
            return new BeneficiaryViewHolder(itemView, beneficiaries);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            BeneficiaryViewHolder viewHolder = holder as BeneficiaryViewHolder;
            viewHolder.Bind(beneficiaries[position]);
        }

        public override int ItemCount => beneficiaries.Count;

        public void UpdateData(List<Beneficiary> beneficiaries)
        {
            this.beneficiaries = beneficiaries;
            NotifyDataSetChanged();
        }
    }

    public class BeneficiaryViewHolder : RecyclerView.ViewHolder
    {
        private TextView nicknameTextView;
        private TextView phoneNumberTextView;
        private Button topUpButton;
        private Context context;
        private List<Beneficiary> beneficiaries;

        public BeneficiaryViewHolder(View itemView, List<Beneficiary> beneficiaries) : base(itemView)
        {
            context = itemView.Context;
            this.beneficiaries = beneficiaries;
            nicknameTextView = itemView.FindViewById<TextView>(Resource.Id.nicknameTextView);
            phoneNumberTextView = itemView.FindViewById<TextView>(Resource.Id.phoneNumberTextView);
            topUpButton = itemView.FindViewById<Button>(Resource.Id.topUpButton);
            topUpButton.Click += TopUpButton_Click;
        }

        private void TopUpButton_Click(object sender, EventArgs e)
        {
            // Retrieve the beneficiary associated with this ViewHolder
            Beneficiary beneficiary = beneficiaries[AdapterPosition];

            // Cast the context to Activity to access the Intent property
            Activity activity = context as Activity;
            if (activity != null)
            {
                // Retrieve user data passed from HomeActivity
                string userDataJson = activity.Intent.GetStringExtra("User");
                User user = JsonConvert.DeserializeObject<User>(userDataJson);
                // Serialize both user and beneficiary objects
                string beneficiaryDataJson = JsonConvert.SerializeObject(beneficiary);


                // Launch TopUpActivity passing the beneficiaryId and user data
                Intent intent = new Intent(context, typeof(TopUpActivity));
                intent.PutExtra("Beneficiary", beneficiaryDataJson);
                intent.PutExtra("User", userDataJson);
                context.StartActivity(intent);
            }
        }



        public void Bind(Beneficiary beneficiary)
        {
            nicknameTextView.Text = beneficiary.Nickname;
            phoneNumberTextView.Text = beneficiary.PhoneNumber;

        }
    }
}
