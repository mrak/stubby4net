using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace stubby.Domain {

    internal class ComparisonUtils {
        public static bool NameValueCollections(NameValueCollection left, NameValueCollection right) {
            if(left == null)
                return right == null;
            if(right == null)
                return false;
            if(left.Count != right.Count)
                return false;

            foreach(var key in left.AllKeys) {
                IList<string> leftValues = left.GetValues(key);
                IList<string> rightValues = right.GetValues(key);

                if(leftValues == null)
                if(rightValues == null)
                    continue;
                else
                    return false;
                if(rightValues == null)
                    return false;

                if(!leftValues.All(rightValues.Contains) || !rightValues.All(leftValues.Contains))
                    return false;
            }
            return true;
        }

        public static bool Lists(IList<string> left, IList<string> right) {
            return left.All(right.Contains) && right.All(left.Contains);
        }
    }
}
