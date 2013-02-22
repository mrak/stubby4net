using System.Collections.Generic;
using System.Runtime.Serialization;
using Compare = stubby.Domain.ComparisonUtils;

namespace stubby.Domain {

   [DataContract]
   public class Request {
      public Request() {
         Method = new List<string> {"GET"};
         Query = new Dictionary<string, string>();
         Headers = new Dictionary<string, string>();
      }

      [DataMember] public string Url { get; set; }
      [DataMember] public IList<string> Method { get; set; }
      [DataMember] public IDictionary<string, string> Headers { get; set; }
      [DataMember] public IDictionary<string, string> Query { get; set; }
      [DataMember] public string Post { get; set; }
      [DataMember] public string File { get; set; }

      protected bool Equals(Request other) {
         if (!string.Equals(Url, other.Url)) return false;
         if (!string.Equals(Post, other.Post)) return false;
         if (!string.Equals(File, other.File)) return false;
         if (!Compare.Lists(Method, other.Method)) return false;
         if (!Compare.Dictionaries(Headers, other.Headers)) return false;
         if (!Compare.Dictionaries(Query, other.Query)) return false;
         return true;
      }

      public bool Matches(Request other) {
         if (Url != other.Url) return false;
         if (!Method.Contains(other.Method[0])) return false;

         foreach (var header in Headers) {
            if (!other.Headers.ContainsKey(header.Key)) return false;
            if (other.Headers[header.Key] != header.Value) return false;
         }
         foreach (var query in Query) {
            if (!other.Query.ContainsKey(query.Key)) return false;
            if (other.Query[query.Key] != query.Value) return false;
         }

         try {
            return System.IO.File.ReadAllText(File) == other.Post;
         } catch {
            if (Post != null && Post != other.Post) return false;
         }

         return true;
      }

      public override int GetHashCode() {
         unchecked {
            var hashCode = (Url != null ? Url.GetHashCode() : 0);
            hashCode = (hashCode*397) ^ (Method != null ? Method.GetHashCode() : 0);
            hashCode = (hashCode*397) ^ (Headers != null ? Headers.GetHashCode() : 0);
            hashCode = (hashCode*397) ^ (Query != null ? Query.GetHashCode() : 0);
            hashCode = (hashCode*397) ^ (Post != null ? Post.GetHashCode() : 0);
            hashCode = (hashCode*397) ^ (File != null ? File.GetHashCode() : 0);
            return hashCode;
         }
      }

      public override bool Equals(object obj) {
         if (ReferenceEquals(null, obj)) return false;
         if (ReferenceEquals(this, obj)) return true;
         return obj.GetType() == GetType() && Equals((Request) obj);
      }

   }

}