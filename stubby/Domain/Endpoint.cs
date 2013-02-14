namespace stubby.Domain {

   internal class Endpoint {
      public Endpoint() {
         Request = new Request();
         Response = new Response();
      }

      public Request Request { get; set; }
      public Response Response { get; set; }

      public override bool Equals(object o) {
         var other = (Endpoint) o;
         return Request.Equals(other.Request);
      }
   }

}