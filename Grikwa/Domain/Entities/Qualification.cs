using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Qualification
    {
        [Key]
        public virtual int QualificationID { get; set; }
        public virtual string Code { get; set; }
        public virtual string FullName { get; set; }
    }
}
