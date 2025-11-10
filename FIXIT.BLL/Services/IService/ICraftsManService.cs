using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.Intrfaces
{
	public interface ICraftsManService
	{
		Task<List<CraftsManDto>> GetAllCraftsMenAsync();
		Task<CraftsManDto> GetCraftsManByIdAsync(int id);
		Task<List<CraftsManDto>> GetCraftsMenByNameAsync(string? fname,string? lname);
		Task CreateCraftsManAsync(CreateCraftsManDto craftsMan);
	    Task<bool>UpdateCraftsManAsync(int id, UpdateCraftsManDto craftsManDto);

		void DeleteCraftsMan(int id);

	}
}
