using FIXIT.BLL.DTOs.CraftsmanDTOs;
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

			return await query.OrderByDescending(c => c.Rating).ToListAsync();
		}
		public async Task<List<CraftsMan>> GetCraftsMenByLocationandServiceAsync(string location, string servicename)
		{
			IQueryable<CraftsMan> query = _dbContext.CraftsMan;
			if (!string.IsNullOrWhiteSpace(location))
			{
				
				query = query.Where(c => EF.Functions.Like(c.Location, $"%{location}%"));

			}
			if (!string.IsNullOrWhiteSpace(location))
			{
				
				query = query.Where(c => c.CraftsManServices
			.Any(cs => EF.Functions.Like(cs.Service.ServiceName, $"%{servicename}%")));

			}

			return await query.OrderByDescending(c => c.Rating).ToListAsync();
		}
		public async Task<CraftsMan> GetCraftsManByEmailAsync(string normalizedEmail)
		{
			//var query = await _dbContext.FindAsync<string>(normalizedEmail);
			return await _dbContext.CraftsMan.FirstOrDefaultAsync(c=> c.NormalizedEmail== normalizedEmail);


			
		}
	}
}
