using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.ReviewDTOs
{
    public class CreateReviewDTO
    {
        public int RatingValue { get; set; }
        public string Comment { get; set; }
        public int ServicesRequestId { get; set; }
    }
}
