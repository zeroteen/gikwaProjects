using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Community
    {
        [Key]
        public virtual long CommunityID { get; set; }
        public virtual string ShortName { get; set; }
        public virtual string LongName { get; set; }
        public virtual string ImageName { get; set; }
    }
}
