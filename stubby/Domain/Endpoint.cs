using System.Runtime.Serialization;

namespace stubby.Domain {

   [DataContract]
   public class Endpoint {
      private Request _request = new Request();
      private Response _response = new Response();

      [DataMember]
      public Request Request {
         get { return _request ?? (_request = new Request()); }
         set { _request = value; }
      }

      [DataMember]
      public Response Response {
         get { return _response ?? (_response = new Response()); }
         set { _response = value; }
      }

      public override bool Equals(object o) {
         var other = (Endpoint) o;
         return Request.Equals(other.Request);
      }
   }
}