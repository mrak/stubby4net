using System.Collections.Generic;
using System.Runtime.Serialization;
using Compare = stubby.Domain.ComparisonUtils;

namespace stubby.Domain {

   [DataContract]
   public class Response {
      public Response() {
         Status = 200;
         Headers = new Dictionary<string, string>();
      }

      [DataMember] public ushort Status { get; set; }
      [DataMember] public IDictionary<string, string> Headers { get; set; }
      [DataMember] public ulong Latency { get; set; }
      [DataMember] public string Body { get; set; }
      [DataMember] public string File { get; set; }

      protected bool Equals(Response other) {
         if (Status != other.Status) return false;
         if (Latency != other.Latency) return false;
         if (!string.Equals(Body, other.Body)) return false;
         if (!string.Equals(File, other.File)) return false;
         if (!Compare.Dictionaries(Headers, other.Headers)) return false;
         return true;
      }

      public override int GetHashCode() {
         unchecked {
            var hashCode = Status.GetHashCode();
            hashCode = (hashCode*397) ^ (Headers != null ? Headers.GetHashCode() : 0);
            hashCode = (hashCode*397) ^ Latency.GetHashCode();
            hashCode = (hashCode*397) ^ (Body != null ? Body.GetHashCode() : 0);
            hashCode = (hashCode*397) ^ (File != null ? File.GetHashCode() : 0);
            return hashCode;
         }
      }

      public override bool Equals(object obj) {
         if (ReferenceEquals(null, obj)) return false;
         if (ReferenceEquals(this, obj)) return true;
         return obj.GetType() == GetType() && Equals((Response) obj);
      }
   }

}