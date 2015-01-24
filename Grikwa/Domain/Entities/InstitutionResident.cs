using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class InstitutionResident
    {
        [Key]
        public virtual int InstitutionResidentID { get; set; }
        public virtual int InstitutionID { get; set; }
        public virtual Community Institution { get; set; }
        public virtual int ResidentID { get; set; }
        public virtual Resident Resident { get; set; }
    }
}
