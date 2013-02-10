using System;
using System.Net;
using stubby.Domain;

namespace stubby.Portals {

   internal class Admin : IDisposable {
      private readonly EndpointDb _endpointDb;
      private readonly HttpListener _listener;

      public Admin(EndpointDb endpointDb) : this(endpointDb, new HttpListener()) {}

      public Admin(EndpointDb endpointDb, HttpListener listener) {
         _endpointDb = endpointDb;
         _listener = listener;
      }

      public void Dispose() {
         _listener.Stop();
      }

      public void Start(string location, uint port) {
         _listener.Prefixes.Add(PortalUtils.BuildUri(location, port));
         _listener.Start();
         _listener.BeginGetContext(AsyncHandler, _listener);
      }

      private void ResponseHandler(HttpListenerContext context) {
         context.Response.Close();
         Console.WriteLine("admin hit");
      }

      private void AsyncHandler(IAsyncResult result) {
         var context = _listener.EndGetContext(result);
         ResponseHandler(context);
         _listener.BeginGetContext(AsyncHandler, _listener);
      }

   }

}