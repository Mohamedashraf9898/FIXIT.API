using Microsoft.AspNetCore.Http;

namespace FIXIT.BLL.DTOs.CraftsmanDTOs
{
   
    public class CraftsManDto
	{
		public string FName { get; set; }
		public string LName { get; set; }
		public string Describtion { get; set; }
		public string? ProfileImage { get; set; }
		public double Rating { get; set; }

	}
}
