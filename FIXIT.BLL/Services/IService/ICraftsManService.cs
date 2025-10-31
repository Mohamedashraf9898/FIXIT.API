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
		Task<IEnumerable<CraftsManDto>> GetAllCraftsMenAsync();
		Task<CraftsManDto> GetCraftsManByIdAsync(int id);
		void CreateCraftsManAsync(CreateCraftsManDto craftsMan);
		bool UpdateCraftsMan(int id, UpdateCraftsManDto craftsManDto);
		void DeleteCraftsMan(int id);

	}
}
