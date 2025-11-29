using FIXIT.BLL.DTOs.ReviewDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.DTOs.CraftsmanDTOs
{
    public class CraftsManDetailsDto
    {
        public CraftsManDto CraftsMan { get; set; }
        public IEnumerable<GetAllReviewsDTO> Reviews { get; set; }
    }
}
