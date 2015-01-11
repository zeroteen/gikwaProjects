using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Product
    {
        public virtual int ProductID { get; set; }
        public virtual string UserID { get; set; }
        //public virtual ApplicationUser User { get; set; }
        public virtual string Name { get; set; }
        public virtual string ShortDescription { get; set; }
        public virtual string LongDescription { get; set; }
        public virtual decimal Price { get; set; }
        public virtual DateTime DatePosted { get; set; }
        public virtual ProductStatus ProductStatus { get; set; }
        public virtual bool EmailNotification { get; set; }
        public virtual byte NumberOfSaleRequests { get; set; }
        public virtual string KeyWords { get; set; }
        public virtual bool AcceptedTerms { get; set; }
        public virtual byte[] FullSizeImage { get; set; }
        public virtual byte[] ThumbnailImage { get; set; }
    }
}
