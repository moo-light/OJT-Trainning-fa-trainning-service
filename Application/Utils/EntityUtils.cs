using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utils
{
    public static class EntityUtils
    {
        public static bool IsWithin<T>(this T value, T minimum, T maximum) where T : IComparable<T>
        {
            if (value.CompareTo(minimum) < 0)
                return false;
            if (value.CompareTo(maximum) > 0)
                return false;
            return true;
        }
    }
}
