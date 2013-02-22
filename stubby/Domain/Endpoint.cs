using System.Runtime.Serialization;

namespace stubby.Domain {

   [DataContract]
   public class Endpoint {
      public Endpoint() {
         Request = new Request();
         Response = new Response();
      }

      [DataMember]
      public Request Request { get; set; }

      [DataMember]
      public Response Response { get; set; }

      protected bool Equals(Endpoint other) {
         return Equals(Request, other.Request) && Equals(Response, other.Response);
      }

      public override int GetHashCode() {
         unchecked {
            return ((Request != null ? Request.GetHashCode() : 0)*397) ^ (Response != null ? Response.GetHashCode() : 0);
         }
      }

      public override bool Equals(object obj) {
         if (ReferenceEquals(null, obj)) return false;
         if (ReferenceEquals(this, obj)) return true;
         return obj.GetType() == GetType() && Equals((Endpoint) obj);
      }

      public bool Matches(Endpoint other) {
         return Request.Matches(other.Request);
      }
   }

}