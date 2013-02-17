using System;
using System.Collections.Generic;
using System.Net;
using stubby.Domain;

namespace stubby.Portals {

   internal class Admin : IDisposable {
      private readonly EndpointDb _endpointDb;
      private readonly HttpListener _listener;
      private readonly IDictionary<string, Action<HttpListenerContext>> _methods;

      public Admin(EndpointDb endpointDb) : this(endpointDb, new HttpListener()) {}

      public Admin(EndpointDb endpointDb, HttpListener listener) {
         _endpointDb = endpointDb;
         _listener = listener;
         _methods = new Dictionary<string, Action<HttpListenerContext>> {
            {"GET", goGET},
            {"HEAD", goGET},
            {"POST", goPOST},
            {"PUT", goPUT},
            {"DELETE", goDELETE}
         };
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
         PortalUtils.PrintIncoming("admin", context.Request.Url.AbsolutePath, context.Request.HttpMethod);
         PortalUtils.AddServerHeader(context.Response);

         if (_methods.ContainsKey(context.Request.HttpMethod)) _methods[context.Request.HttpMethod](context);
         else goInvalid(context);

         context.Response.Close();
         PortalUtils.PrintOutgoing("admin", context.Request.Url.AbsolutePath, context.Response.StatusCode);
      }

      private void goGET(HttpListenerContext context) {}
      private void goPOST(HttpListenerContext context) {}
      private void goPUT(HttpListenerContext context) {}
      private void goDELETE(HttpListenerContext context) {}

      private void goInvalid(HttpListenerContext context) {
         context.Response.StatusCode = (int) HttpStatusCode.MethodNotAllowed;
         context.Response.AddHeader("Allow", "GET, HEAD, POST, PUT, DELETE");
      }

      private void AsyncHandler(IAsyncResult result) {
         var context = _listener.EndGetContext(result);
         ResponseHandler(context);
         _listener.BeginGetContext(AsyncHandler, _listener);
      }
   }

}