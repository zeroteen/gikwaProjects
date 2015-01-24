using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class MeetPlace
    {
        [Key]
        public virtual int MeetPlaceID { get; set; }
        public virtual int InstitutionID { get; set; }
        public virtual Community Institution { get; set; }
        public virtual string Name { get; set; }
    }
}
