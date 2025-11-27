using Microsoft.AspNetCore.Http;

namespace FIXIT.BLL.DTOs.CraftsmanDTOs
{
   
    public class CraftsManDto
	{
        public int Id { get; set; }
        public string FName { get; set; }
		public string LName { get; set; }
		public string Describtion { get; set; }
		public string? ProfileImage { get; set; }
		public double Rating { get; set; }
		public string Location { get; set; }
		public bool IsVerified { get; set; }
		public string NormalizedEmail { get; set; }
        public string ServiceName { get; set; }
        public int ExperienceOfYears { get; set; }
        public decimal HourlyRate { get; set; }
        public string PhoneNumber { get; set; }

        public string NationalId { get; set; }

    }
}
