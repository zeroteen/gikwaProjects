using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Faculty
    {
        [Key]
        public virtual int FacultyID { get; set; }
        public virtual string Name { get; set; }
    }
}
