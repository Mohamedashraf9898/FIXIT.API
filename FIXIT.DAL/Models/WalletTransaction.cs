namespace FIXIT.DAL.Models
{
     public enum TransactionType { 
    
        instapay,
        ewallet,
        credit

    }
    public enum Transactionmethod { 
    
    Withdraw,
    Deposits
    }
    public class WalletTransaction
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public int? ServiceRequestId { get; set; }
        public decimal? Amount { get; set; }
        public Transactionmethod? Transactionmethod { get; set; }
        public TransactionType? Transactiontype { get; set; } //for WithdrawFunds only
        public string? TransationInfo { get; set; } //for WithdrawFunds only
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual Wallet Wallet { get; set; }
        public virtual ServicesRequest ServicesRequest { get; set; }
        public bool? ispayed { get; set; }=false;
    }
}
