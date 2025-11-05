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
    public class WalletRepository : GenericRepository<Wallet>, IWalletRepository
    {
        private readonly FixItDbContext _context;
        public WalletRepository(FixItDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<Wallet> GetWalletByCraftsManIdAsync(int craftsManId)
        {
            return await _context.Wallets
                .FirstOrDefaultAsync(w => w.CraftsManId == craftsManId);
        }

    }
}
