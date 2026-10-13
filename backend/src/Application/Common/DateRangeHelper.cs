using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
   public class DateRangeHelper
    {
        public static bool Overlaps(DateOnly aStart,DateOnly aEnd,DateOnly bStart, DateOnly bEnd)
        {
            return aStart <= bEnd && bStart <= aEnd;
        }
    }
}
