using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Major
    {
        [Key]
        public virtual int MajorID { get; set; }
        public virtual string Name { get; set; }
    }
}
