using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Section : BaseEntity
    {
        public string Name { get; set; }=string.Empty;
        public string Color { get; set; }=string.Empty;
    }
}
