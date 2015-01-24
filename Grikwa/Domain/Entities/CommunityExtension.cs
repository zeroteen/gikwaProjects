using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CommunityExtension
    {
        public virtual long CommunityEmailExtentionID { get; set; }
        public virtual long CommunityID { get; set; }
        public virtual Community Community { get; set; }
        public virtual string Extension { get; set; }
    }
}
