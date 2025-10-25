
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
		void CreateCraftsManAsync(CraftsMan craftsMan);
		void UpdateCraftsManAsync(int id, UpdateCraftsManDto craftsManDto);
		void DeleteCraftsManAsync(int id);

	}
}
