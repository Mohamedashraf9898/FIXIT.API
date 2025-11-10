using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.DAL;
using FIXIT.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace FIXIT.BLL.Repositories.Repo
{
    public class WalletTransactionRepository : GenericRepository<WalletTransaction>, IWalletTransactionRepository
    {
        private readonly FixItDbContext _context;

        public WalletTransactionRepository(FixItDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<WalletTransaction> GetByServiceRequestIdAsync(int serviceRequestId)
        {
            return await _context.WalletTransactions.FirstOrDefaultAsync(t => t.ServiceRequestId == serviceRequestId);
        }

        public async Task<IEnumerable<WalletTransaction>> GetAllByWalletIdAsync(int walletId)
        {
            return await _context.WalletTransactions
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}
