using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Token
    {
        [Key]
        public string Id { get; set; }
        public string UserID { get; set; }
        public User User { get; set; }
        public DateTime IssueDate { get; set; }
    }
}
