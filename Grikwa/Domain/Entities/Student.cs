using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Student : User
    {
        public int QualificationID { get; set; }
        public Qualification Qualification { get; set; }

        public int InstitutionID { get; set; }
        public Community Institution { get; set; }

        public int FacultyID { get; set; }
        public Faculty Faculty { get; set; }

        public int ResidentID { get; set; }
        public Resident Resident { get; set; }
    }
}
