using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.DAL.Models
{

    public class BaseEntity
    {
        public int Id { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string NationalId { get; set; }
        public string Location { get; set; }
        public string PhoneNumber { get; set; }
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        
    }
}
