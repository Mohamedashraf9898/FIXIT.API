using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.ReviewDTOs
{
    public class GetAllReviewsDTO
    {
        public int Id { get; set; }
        public int RatingValue { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public int ClientId { get; set; }
        public int CraftsManId { get; set; }
        public int ServicesRequestId { get; set; }
    }
}
