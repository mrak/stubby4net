using System;
using System.Collections.Generic;
using System.Net;
using stubby.Domain;

namespace stubby.Portals {

   internal class Admin : IDisposable {
      private const string Name = "admin";
      private const string Ping = "/ping";
      private const string PlainText = "text/plain";
      private const string AllowedMethods = "GET, HEAD, POST, PUT, DELETE";
      private static readonly byte[] Pong = PortalUtils.GetBytes("pong");
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

         PortalUtils.PrintListening(Name, location, port);
      }

      private void ResponseHandler(HttpListenerContext context) {
         PortalUtils.PrintIncoming(Name, context.Request.Url.AbsolutePath, context.Request.HttpMethod);
         PortalUtils.AddServerHeader(context.Response);

         if (_methods.ContainsKey(context.Request.HttpMethod)) _methods[context.Request.HttpMethod](context);
         else goInvalid(context);

         context.Response.Close();
         PortalUtils.PrintOutgoing(Name, context.Request.Url.AbsolutePath, context.Response.StatusCode);
      }

      private void goGET(HttpListenerContext context) {
         var url = context.Request.Url.AbsolutePath;

         if (url.Equals(Ping)) {
            goPing(context);
            return;
         }
      }

      private void goPOST(HttpListenerContext context) {}
      private void goPUT(HttpListenerContext context) {}
      private void goDELETE(HttpListenerContext context) {}

      private void goInvalid(HttpListenerContext context) {
         context.Response.StatusCode = (int) HttpStatusCode.MethodNotAllowed;
         context.Response.AddHeader("Allow", AllowedMethods);
      }

      private void goPing(HttpListenerContext context) {
         context.Response.StatusCode = (int) HttpStatusCode.OK;
         context.Response.ContentType = PlainText;
         context.Response.Close(Pong, false);
      }

      private void AsyncHandler(IAsyncResult result) {
         var context = _listener.EndGetContext(result);
         ResponseHandler(context);
         _listener.BeginGetContext(AsyncHandler, _listener);
      }
   }

}