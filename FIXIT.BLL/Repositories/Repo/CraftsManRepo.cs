using FIXIT.BLL.Repositories.IRepo;
using FIXIT.DAL;
using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Repositories.Repo
{
	public class CraftsManRepo : GenericRepository<CraftsMan>, ICraftsManRepo
	{
		private readonly FixItDbContext dbContext;

		public CraftsManRepo(FixItDbContext dbContext) : base(dbContext)
		{
			this.dbContext = dbContext;
		}

		public CraftsMan GetCraftsManByName(string fName, string lName)
		{
			return dbContext.CraftsMan.FirstOrDefault(c => c.FName == fName && c.LName == lName);
		}
	}
}
