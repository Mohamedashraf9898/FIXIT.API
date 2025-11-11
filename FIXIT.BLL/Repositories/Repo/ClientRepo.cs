using FIXIT.BLL.Repositories.IRepo;
using FIXIT.DAL;
using FIXIT.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace FIXIT.BLL.Repositories.Repo
{
    public class ClientRepo : GenericRepository<Client>, IClientRepo
    {
        private readonly FixItDbContext dbContext;

        public ClientRepo(FixItDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Client> GetClientByEmailAsync(string email)
        {
     
        
            //var query = await _dbContext.FindAsync<string>(normalizedEmail);
            return await dbContext.Clients.FirstOrDefaultAsync(c => c.NormalizedEmail == email);



        }
    }
 }

