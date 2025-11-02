using FIXIT.BLL.Repositories.IRepo;
using FIXIT.DAL;
using FIXIT.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Repositories.Repo
{
	public class CraftsManRepo : GenericRepository<CraftsMan>, ICraftsManRepo
	{
		private readonly FixItDbContext _dbContext;

		public CraftsManRepo(FixItDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<CraftsMan>> GetCraftsManByNameAsync(string? fName, string? lName)
		{
			IQueryable<CraftsMan> query = _dbContext.CraftsMan;

			if (!string.IsNullOrWhiteSpace(fName))
				query = query.Where(c => EF.Functions.Like(c.FName, $"%{fName}%"));

			if (!string.IsNullOrWhiteSpace(lName))
				query = query.Where(c => EF.Functions.Like(c.LName, $"%{lName}%"));

			return await query.ToListAsync();
		}
	}
}
