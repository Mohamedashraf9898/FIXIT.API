using FIXIT.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.CraftsmanDTOs
{
	public class CreateCraftsManDto
	{
		public string FName { get; set; }
		public string LName { get; set; }
		public string NationalId { get; set; }
		public string Location { get; set; }
		public string PhoneNumber { get; set; }
		public Gender Gender { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string Describtion { get; set; }
		public string ProfileImage { get; set; }
		public int ExperienceOfYears { get; set; }
		public decimal HourlyRate { get; set; }



	}
}
