using System;
using System.Net;
using stubby.Domain;

namespace stubby.Portals {

   public class Stubs {
      private readonly EndpointDb _endpointsDb;
      private readonly Portal _portal;

      public Stubs(EndpointDb endpointsDb) : this(endpointsDb, new Portal(new HttpListener())) {}

      public Stubs(EndpointDb endpointsDb, Portal portal) {
         _endpointsDb = endpointsDb;
         _portal = portal;
      }

      public void Start(string location, uint port) {
         _portal.Start(location, port);
      }

      public void Listen() {
         _portal.RespondWith(RequestHandler);
      }

      private void RequestHandler(object incoming) {
         var context = (HttpListenerContext) incoming;

         context.Response.Close();
         Console.WriteLine("stubs hit");
      }
   }

}