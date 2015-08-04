using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Entities
{
    public class Category
    {
        // hello
        public virtual int CategoryID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }
        public virtual string IconName { get; set; }
    }
}
