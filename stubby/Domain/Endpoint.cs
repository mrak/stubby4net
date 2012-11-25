namespace stubby.Domain {

   public class Endpoint {
      public Endpoint() {
         Request = new Request();
         Response = new Response();
      }

      public Request Request { get; set; }
      public Response Response { get; set; }
   }

}