using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        [MaxLength(6)]
        public string Intials { get; set; }

        [MaxLength(50)]
        public string Surname { get; set; }

        [MaxLength(10)]
        public string TitleID { get; set; }
        public Title Title { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string OptionalEmail { get; set; }

        public bool Verified { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime LastSeen { get; set; }

        public virtual string ProfileImageName { get; set; }
    }
}
