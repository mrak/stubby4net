using System.Collections.Generic;
using System.Linq;

namespace stubby.Domain
{
   internal class ComparisonUtils
   {
      public static bool Dictionaries(IDictionary<string, string> left, IDictionary<string, string> right) {
         foreach (var o in right) {
            string retreived;
            if (!left.TryGetValue(o.Key, out retreived)) return false;
            if (!Equals(retreived, o.Value)) return false;
         }
         foreach (var o in left) {
            string retreived;
            if (!right.TryGetValue(o.Key, out retreived)) return false;
            if (!Equals(retreived, o.Value)) return false;
         }
         return true;
      }

      public static bool Lists(IList<string> left, IList<string> right) {
         return left.All(right.Contains) && right.All(left.Contains);
      }
   }
}
