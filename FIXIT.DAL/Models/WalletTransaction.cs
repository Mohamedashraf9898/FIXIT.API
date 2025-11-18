namespace FIXIT.DAL.Models
{
    #region Gehad
  

    public enum TransactionType
    {
        Deposit,
        Income,
        ServicePayment, 
        WithdrawalPending,
        WithdrawalSuccess,
        WithdrawalFailed
    }
    #endregion
    public class WalletTransaction
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public int? ServiceRequestId { get; set; }

        
        public decimal Amount { get; set; }
        // public string TransactionType { get; set; } // "Credit", "Debit", "Commission"
        // public string Description { get; set; }
        #region Gehad
        public int CraftsManId { get; set; }
        public DateTime TransactionDate { get; set; }

        public TransactionType Type { get; set; }
        public string Status { get; set; } = "Pending";
        public string? ExternalTransactionId { get; set; }
        public string? Reason { get; set; }
        #endregion
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual Wallet Wallet { get; set; }

        public virtual ServicesRequest ServicesRequest { get; set; }
    }
}
