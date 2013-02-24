using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using Compare = stubby.Domain.ComparisonUtils;

namespace stubby.Domain {

   [DataContract]
   public class Request {
      public Request() {
         Method = new List<string> {"GET"};
         Query = new NameValueCollection();
         Headers = new NameValueCollection();
      }

      [DataMember] public string Url { get; set; }
      [DataMember] public IList<string> Method { get; set; }
      [DataMember] public NameValueCollection Headers { get; set; }
      [DataMember] public NameValueCollection Query { get; set; }
      [DataMember] public string Post { get; set; }
      [DataMember] public string File { get; set; }

      protected bool Equals(Request other) {
         if (!string.Equals(Url, other.Url)) return false;
         if (!string.Equals(Post, other.Post)) return false;
         if (!string.Equals(File, other.File)) return false;
         if (!Compare.Lists(Method, other.Method)) return false;
         if (!Compare.NameValueCollections(Headers, other.Headers)) return false;
         if (!Compare.NameValueCollections(Query, other.Query)) return false;
         return true;
      }

      public bool Matches(Request other) {
         if (Url != other.Url) return false;
         if (!Method.Contains(other.Method[0])) return false;

         foreach (var header in Headers.AllKeys) {
            IList<string> otherValues = other.Headers.GetValues(header);
            if (otherValues == null) return false;
            if (!Headers.GetValues(header).All(otherValues.Contains)) return false;
         }

         foreach (var variable in Query.AllKeys) {
            IList<string> otherValues = other.Query.GetValues(variable);
            if (otherValues == null) return false;
            if (!Query.GetValues(variable).All(otherValues.Contains)) return false;
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