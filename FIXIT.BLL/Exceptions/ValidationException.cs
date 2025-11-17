using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIXIT.API.Erorrs.Exceptions;
using FIXIT.API.Erorrs;

namespace FIXIT.BLL.Exceptions
{
    public class ValidationException : BadRequestException
    {
        //public required IEnumerable<ValditonErorr> Erorrs { get; set; }
        public ValidationException(string message = "Bad Request") : base(message)
        {

        }
        
    }
}
