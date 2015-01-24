using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserMajor
    {
        public virtual int UserMajorID { get; set; }
        public virtual string UserID { get; set; }
        public virtual User User { get; set; }
        public virtual int MajorID { get; set; }
        public virtual Major Major { get; set; }
    }
}
