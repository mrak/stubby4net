using System;
using System.Net;
using stubby.Domain;

namespace stubby.Portals {

   public class Admin {
      private readonly EndpointDb _endpointDb;
      private readonly Portal _portal;

      public Admin(EndpointDb endpointDb) : this(endpointDb, new Portal()) {}

      public Admin(EndpointDb endpointDb, Portal portal) {
         _endpointDb = endpointDb;
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
         Console.WriteLine("admin hit");
      }
   }

}