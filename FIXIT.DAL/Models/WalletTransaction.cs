namespace FIXIT.DAL.Models
{
    public class WalletTransaction
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public int? ServiceRequestId { get; set; }
        public decimal Amount { get; set; }
       // public string TransactionType { get; set; } // "Credit", "Debit", "Commission"
       // public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual Wallet Wallet { get; set; }
        public virtual ServicesRequest ServicesRequest { get; set; }
    }
}
