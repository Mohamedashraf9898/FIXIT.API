using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.CraftsmanDTOs
{
	public class UpdateCraftsManDto
	{
		public int Id { get; set; }
		public string FName { get; set; }
		public string LName { get; set; }
		public string Describtion { get; set; }
        public string PhoneNumber { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public IFormFile? NationalIdPic { get; set; } // For upload only
		public int ExperienceOfYears { get; set; }
		public decimal HourlyRate { get; set; }
        public bool IsVerified { get; set; }
    }
}
