using FIXIT.BLL.Interfaces;
using FIXIT.DAL;
using FIXIT.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Repositories
{
	public class CraftsManRepo : GenericRepository<CraftsMan>, ICraftsManRepo
	{
		private readonly FixItDbContext dbContext;

		public CraftsManRepo(FixItDbContext dbContext) : base(dbContext)
		{
			this.dbContext = dbContext;
		}

		public async Task<CraftsMan> GetCraftsManByName(string fName, string lName)
		{
			return await dbContext.CraftsMan.FirstOrDefaultAsync(c => c.FName == fName && c.LName == lName);
		}
	}
}
