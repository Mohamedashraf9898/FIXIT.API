using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs
{
	public class UpdateCraftsManDto
	{
		public int Id { get; set; }
		public string FName { get; set; }
		public string LName { get; set; }
		public string Describtion { get; set; }
		public string ProfileImage { get; set; }
		public int ExperienceOfYears { get; set; }
		public decimal HourlyRate { get; set; }


	}
}
