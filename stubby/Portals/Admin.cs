using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using stubby.Domain;

namespace stubby.Portals {

   internal class Admin : IDisposable {
      private const string Name = "admin";
      private const string PingUrl = "/ping";
      private const string StatusUrl = "/status";
      private const string Html = "text/html";
      private const string Root = "/";
      private const string AllowedMethods = "GET, HEAD, POST, PUT, DELETE";
      private const string AcceptableUrls = @"^\/(ping|status|[0-9]*)$";
      private static readonly byte[] Pong = PortalUtils.GetBytes("pong");
      private readonly EndpointDb _endpointDb;
      private readonly HttpListener _listener;
      private readonly IDictionary<string, Action<HttpListenerContext>> _methods;

      public Admin(EndpointDb endpointDb) : this(endpointDb, new HttpListener()) {}

      public Admin(EndpointDb endpointDb, HttpListener listener) {
         _endpointDb = endpointDb;
         _listener = listener;
         _methods = new Dictionary<string, Action<HttpListenerContext>> {
            {"GET", GoGet},
            {"HEAD", GoGet},
            {"POST", GoPost},
            {"PUT", GoPUT},
            {"DELETE", GoDelete}
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

      public void Stop() {
         _listener.Stop();
      }

      private void ResponseHandler(HttpListenerContext context) {
         PortalUtils.PrintIncoming(Name, context.Request.Url.AbsolutePath, context.Request.HttpMethod);
         PortalUtils.AddServerHeader(context.Response);

         if (!Regex.IsMatch(context.Request.Url.AbsolutePath, AcceptableUrls)) GoInvalid(context);
         else if (_methods.ContainsKey(context.Request.HttpMethod)) _methods[context.Request.HttpMethod](context);
         else GoInvalid(context);

         context.Response.Close();
         PortalUtils.PrintOutgoing(Name, context.Request.Url.AbsolutePath, context.Response.StatusCode);
      }

      private void GoGet(HttpListenerContext context) {
         var url = context.Request.Url.AbsolutePath;

         if (url.Equals(PingUrl)) {
            GoPing(context);
            return;
         }

         if (url.Equals(StatusUrl)) {
            GoStatus(context);
            return;
         }

         if (url.Equals(Root)) {
            var all = _endpointDb.Fetch();
            PortalUtils.SerializeToJson(all, context.Response);
            return;
         }

         var id = uint.Parse(url.Substring(1));
         var endpoint = _endpointDb.Fetch(id);

         if (endpoint == null) {
            GoNotFound(context);
            return;
         }

         if (context.Request.HttpMethod.Equals("GET")) PortalUtils.SerializeToJson(endpoint, context.Response);
      }

      private static void GoPost(HttpListenerContext context) {}
      private static void GoPUT(HttpListenerContext context) {}
      private static void GoDelete(HttpListenerContext context) {}

      private static void GoInvalid(HttpListenerContext context) {
         context.Response.StatusCode = (int) HttpStatusCode.MethodNotAllowed;
         context.Response.AddHeader("Allow", AllowedMethods);
      }

      private static void GoNotFound(HttpListenerContext context) {
         context.Response.StatusCode = (int) HttpStatusCode.NotFound;
      }

      private static void GoPing(HttpListenerContext context) {
         context.Response.StatusCode = (int) HttpStatusCode.OK;
         context.Response.OutputStream.Write(Pong, 0, Pong.Length);
      }

      private static void GoStatus(HttpListenerContext context) {
         context.Response.StatusCode = (int) HttpStatusCode.OK;
         context.Response.ContentType = Html;
      }

      private void AsyncHandler(IAsyncResult result) {
         var context = _listener.EndGetContext(result);
         ResponseHandler(context);
         _listener.BeginGetContext(AsyncHandler, _listener);
      }
   }

}