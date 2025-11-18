using FIXIT.BLL.Repositories.IRepo;
using FIXIT.DAL;
using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FIXIT.BLL.Repositories.Repo
{
    public class WithdrawalRepository : GenericRepository<WithdrawalRequest>, IWithdrawalRepository
    {
        private readonly FixItDbContext _dbContext;
        public WithdrawalRepository(FixItDbContext db) : base(db)
        {
            _dbContext = db;
        }

        public async Task<List<WithdrawalRequest>> GetByCraftsManIdAsync(int craftsManId)
        {
            return await _dbContext.WithdrawalRequests.Where(w => w.CraftsManId == craftsManId).ToListAsync();
        }

        public async Task<WithdrawalRequest?> GetPendingByIdAsync(int id)
        {
            return await _dbContext.WithdrawalRequests.FirstOrDefaultAsync(w => w.Id == id && w.Status == WithdrawalStatus.Pending);
        }
    }

}
