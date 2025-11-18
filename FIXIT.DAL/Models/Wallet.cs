namespace FIXIT.DAL.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        public int CraftsManId { get; set; }
        public decimal Balance { get; set; } = 0;
        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;
        public bool IsWithdrawEnabled { get; set; } = true; //Gehad

        public virtual CraftsMan CraftsMan { get; set; }
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();

    }
}
