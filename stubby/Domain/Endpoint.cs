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

      public override bool Equals(object o) {
         var other = (Endpoint) o;
         return Request.Equals(other.Request);
      }
   }
}