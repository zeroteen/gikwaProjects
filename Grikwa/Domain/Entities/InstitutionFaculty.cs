using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class InstitutionFaculty
    {
        [Key]
        public virtual int InstitutionFacultyID { get; set; }
        public virtual int InstitutionID { get; set; }
        public virtual int FacultyID { get; set; }
        public virtual Community Institution { get; set; }
        public virtual Faculty Faculty { get; set; }
    }
}
