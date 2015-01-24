using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class InstitutionFacultyQualificationMajor
    {
        [Key]
        public virtual int InstitutionFacultyQualificationMajorID { get; set; }
        public virtual int InstitutionFacultyQualificationID { get; set; }
        public virtual int MajorID { get; set; }
        public virtual Major Major { get; set; }
        public virtual InstitutionFacultyQualification InstitutionFacultyQualification { get; set; }
    }
}
