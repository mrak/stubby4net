using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using stubby.CLI;
using stubby.Domain;
using utils = stubby.Portals.PortalUtils;

namespace stubby.Portals {

   internal class Admin : IDisposable {
      private const string Name = "admin";
      private const string PingUrl = "/ping";
      private const string StatusUrl = "/status";
      private const string Root = "/";
      private const string IdUrls = @"^\/[1-9][0-9]*$";
      private const int UnprocessableEntity = 422;
      private const string UnprocessableEntityMessage = "The request was well-formed but was unable to be followed due to semantic errors.";
      private const string Pong = "pong";
      private readonly EndpointDb _endpointDb;
      private readonly HttpListener _listener;
      private readonly IDictionary<string, Action<HttpListenerContext>> _idmethods;
      private readonly IDictionary<string, Action<HttpListenerContext>> _rootmethods;
      private readonly IDictionary<string, Action<HttpListenerContext>> _pingmethods;
      private readonly IDictionary<string, Action<HttpListenerContext>> _statusmethods;

      public Admin(EndpointDb endpointDb) : this(endpointDb, new HttpListener()) {}

      public Admin(EndpointDb endpointDb, HttpListener listener) {
         _endpointDb = endpointDb;
         _listener = listener;
         _pingmethods = new Dictionary<string, Action<HttpListenerContext>> {
            {"GET", GoPing},
            {"HEAD", GoPing}
         };
         _statusmethods = new Dictionary<string, Action<HttpListenerContext>> {
            {"GET", GoStatus},
            {"HEAD", GoStatus}
         };
         _idmethods = new Dictionary<string, Action<HttpListenerContext>> {
            {"GET", GoGet},
            {"HEAD", GoGet},
            {"PUT", GoPut},
            {"DELETE", GoDelete}
         };
         _rootmethods = new Dictionary<string, Action<HttpListenerContext>> {
            {"GET", GoGetAll},
            {"HEAD", GoGetAll},
            {"POST", GoPost},
            {"DELETE", GoDeleteAll}
         };
      }

      public void Dispose() {
         _listener.Stop();
      }

      public void Start(string location, uint port) {
         _listener.Prefixes.Add(utils.BuildUri(location, port));
         _listener.Start();
         _listener.BeginGetContext(AsyncHandler, _listener);

         utils.PrintListening(Name, location, port);
      }

      public void Stop() {
         _listener.Stop();
      }

      private void ResponseHandler(HttpListenerContext context) {
         var url = context.Request.Url.AbsolutePath;
         IDictionary<string, Action<HttpListenerContext>> methods;
         Action<HttpListenerContext> action;

         if (url.Equals(PingUrl)) methods = _pingmethods;
         else if (url.Equals(StatusUrl)) methods = _statusmethods;
         else if (url.Equals(Root)) methods = _rootmethods;
         else if (Regex.IsMatch(url, IdUrls)) methods = _idmethods;
         else { utils.SetStatus(context, HttpStatusCode.NotFound); return; }

         if (methods.TryGetValue(context.Request.HttpMethod, out action)) action(context);
         else GoInvalid(context, methods.Keys);
      }

      private void GoGetAll(HttpListenerContext context) {
         var all = _endpointDb.Fetch();
         if (context.Request.HttpMethod.Equals("GET"))
            utils.SerializeToJson(all, context);
      }

      private void GoGet(HttpListenerContext context) {
         var id = uint.Parse(context.Request.Url.AbsolutePath.Substring(1));
         var endpoint = _endpointDb.Fetch(id);

         if (endpoint == null) {
            utils.SetStatus(context, HttpStatusCode.NotFound);
            return;
         }

         if (context.Request.HttpMethod.Equals("GET")) utils.SerializeToJson(endpoint, context);
      }

      private void GoPost(HttpListenerContext context) {
         var data = utils.ReadPost(context.Request);
         IList<Endpoint> parsed;

         try {
            parsed = YamlParser.FromString(data);
         } catch {
            utils.SetStatus(context, HttpStatusCode.BadRequest);
            return;
         }

         if (parsed.Count != 1) {
            Unprocessable(context);
            return;
         }

         uint id;
         _endpointDb.Insert(parsed[0], out id);
         utils.AddLocationHeader(context, id);
         utils.SetStatus(context, HttpStatusCode.Created);
      }

      private void GoPut(HttpListenerContext context) {
         var id = uint.Parse(context.Request.Url.AbsolutePath.Substring(1));

         if (_endpointDb.Fetch(id) == null) {
            utils.SetStatus(context, HttpStatusCode.NotFound);
            return;
         }

         var data = utils.ReadPost(context.Request);
         IList<Endpoint> parsed;

         try {
            parsed = YamlParser.FromString(data);
         } catch {
            utils.SetStatus(context, HttpStatusCode.BadRequest);
            return;
         }

         if (parsed.Count != 1) {
            Unprocessable(context);
            return;
         }

         _endpointDb.Replace(id, parsed[0]);
      }

      private void GoDeleteAll(HttpListenerContext context) {
         _endpointDb.Delete();
         utils.SetStatus(context, HttpStatusCode.NoContent);
      }

      private void GoDelete(HttpListenerContext context) {
         var id = uint.Parse(context.Request.Url.AbsolutePath.Substring(1));

         if (_endpointDb.Delete(id)) utils.SetStatus(context, HttpStatusCode.NoContent);
         else utils.SetStatus(context, HttpStatusCode.NotFound);
      }

      private static void GoInvalid(HttpListenerContext context, IEnumerable<string> methods) {
         utils.SetStatus(context, HttpStatusCode.MethodNotAllowed);
         var allowedMethods = methods.Aggregate("", (current, method) => current + (" " + method)).Trim();
         context.Response.Headers.Add(HttpResponseHeader.Allow, allowedMethods);
      }

      private static void Unprocessable(HttpListenerContext context) {
         utils.SetStatus(context, UnprocessableEntity);
         utils.WriteBody(context, UnprocessableEntityMessage);
      }

      private static void GoPing(HttpListenerContext context) {
         utils.WriteBody(context, Pong);
      }

      private static void GoStatus(HttpListenerContext context) {
         utils.SetHtmlType(context);
      }

      private void AsyncHandler(IAsyncResult result) {
         var context = _listener.EndGetContext(result);

         utils.PrintIncoming(Name, context);
         utils.SetServerHeader(context);
         utils.SetJsonType(context);

         ResponseHandler(context);

         context.Response.Close();
         utils.PrintOutgoing(Name, context);

         _listener.BeginGetContext(AsyncHandler, _listener);
      }
   }

}