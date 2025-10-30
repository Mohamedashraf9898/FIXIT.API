
using FIXIT.BLL.DTOs;
using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services
{
	public interface ICraftsManService
	{
		Task<IEnumerable<CraftsManDto>> GetAllCraftsMenAsync();
		Task<CraftsManDto> GetCraftsManByIdAsync(int id);
		Task<CraftsManDto> GetCraftsByNameAsync(string name,string lname);
		void CreateCraftsManAsync(CreateCraftsManDto craftsMan);
		void UpdateCraftsMan(int id, UpdateCraftsManDto craftsManDto);
		void DeleteCraftsMan(int id);

	}
}
