using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class InstitutionFacultyQualification
    {
        [Key]
        public virtual int InstitutionFacultyQualificationID { get; set; }
        public virtual int IFID { get; set; }
        public virtual int QualificationID { get; set; }
        public virtual Qualification Qualification { get; set; }
        public virtual InstitutionFaculty InstitutionFaculty { get; set; }
    }
}
