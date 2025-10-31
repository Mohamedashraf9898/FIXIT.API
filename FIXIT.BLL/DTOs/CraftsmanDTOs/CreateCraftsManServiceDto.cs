using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.CraftsmanDTOs
{
	public class CreateCraftsManServiceDto
	{
		public int CraftsManId { get; set; }
		public int ServiceId { get; set; }
		public decimal HourlyRate { get; set; }
	}
}
