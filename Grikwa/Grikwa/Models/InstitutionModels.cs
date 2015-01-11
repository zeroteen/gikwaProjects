using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Grikwa.Models
{

    /// <summary>
    /// The Institution class to store the institution where the student studies
    /// e.g UCT, UWC,
    /// </summary>
    public class Institution
    {
        [Key]
        public virtual int InstitutionID { get; set; }
        public virtual string Name { get; set; }
        public virtual string abbreviation{ get; set; }
        public virtual string Extension1 { get; set; }
        public virtual string Extension2 { get; set; }
        public virtual string Extension3 { get; set; }
        public virtual string Extension4 { get; set; }
        public virtual string Extension5 { get; set; }
        public virtual byte[] Image { get; set; }
    }

    public class MeetPlace
    {
        [Key]
        public virtual int MeetPlaceID { get; set; }
        public virtual int InstitutionID { get; set; }
        public virtual Institution Institution { get; set; }
        public virtual string Name { get; set; }
    }

    /// <summary>
    /// The Resident class to store the resident where the student stays
    /// </summary>
    public class Resident
    {
        [Key]
        public virtual int ResidentID { get; set; }
        public virtual string Name { get; set; }
    }

    /// <summary>
    /// The faculty the student is in
    /// </summary>
    public class Faculty
    {
        [Key]
        public virtual int FacultyID { get; set; }
        
        public virtual string Name { get; set; }
        
    }

    /// <summary>
    /// The major the student is taking 
    /// </summary>
    public class Major
    {
        [Key]
        public virtual int MajorID {get; set; }
        
        public virtual string Name { get; set; }
        
    }

    /// <summary>
    /// The qualification of the student
    /// e.g BSc, Bcom, NDip
    /// </summary>
    public class Qualification
    {

        /// <summary>
        /// The id of the qualification. This is for database purposes
        /// </summary>
        [Key]
        public virtual int QualificationID {get; set; }

        /// <summary>
        /// The code of the qualification e.g. BSc, Bcom, NDip
        /// </summary>
        public virtual string Code { get; set; }
        
        /// <summary>
        /// The full name of the qualification e.g. Bachelor of Science
        /// </summary>
        public virtual string FullName { get; set; }
    }

    public class Institution_Resident
    {
        [Key]
        public virtual int IRID { get; set; }
        public virtual int InstitutionID { get; set; }
        public virtual Institution Institution { get; set; }
        public virtual int ResidentID { get; set; }
        public virtual Resident Resident { get; set; }
    }

    public class Institution_Faculty
    {
        [Key]
        public virtual int IFID {get; set; }

        public virtual int InstitutionID { get; set; }
        public virtual int FacultyID { get; set; }
        public virtual Institution Institution { get; set; }
        public virtual Faculty Faculty { get; set; }

    }

    public class Institution_Faculty_Qualification
    {

        [Key]
        public virtual int IFQID { get; set; }

        public virtual int IFID { get; set; }
        public virtual int QualificationID { get; set; }
        public virtual Qualification Qualification { get; set; }
        public virtual Institution_Faculty Institution_Faculty { get; set; }
        
    }

    public class Institution_Faculty_Qualification_Major
    {
        [Key]
        public virtual int IFQMID { get; set; }

        public virtual int IFQID { get; set; }
        public virtual int MajorID { get; set; }
        public virtual Major Major { get; set; }
        public virtual Institution_Faculty_Qualification Institution_Faculty_Qualification { get; set; }

    }

}