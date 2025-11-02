using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Repositories.IRepo
{
	public interface ICraftsManRepo : IGenericRepository<CraftsMan>
	{
		
		Task<List<CraftsMan>> GetCraftsManByNameAsync(string? fName, string? lName);
	}
}
