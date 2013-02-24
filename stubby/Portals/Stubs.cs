using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using stubby.Domain;
using utils = stubby.Portals.PortalUtils;

namespace stubby.Portals {

   internal class Stubs : IPortal {
      private const string Name = "stubs";
      private const string UnregisteredEndoint = "is not a registered endpoint";
      private const string UnexpectedError = "unexpectedtly generated a server error";
      private readonly EndpointDb _endpointDb;
      private readonly HttpListener _listener;

      public Stubs(EndpointDb endpointDb) : this(endpointDb, new HttpListener()) {}

      public Stubs(EndpointDb endpointDb, HttpListener listener) {
         _endpointDb = endpointDb;
         _listener = listener;
      }

      public void Dispose() {
         _listener.Stop();
      }

      public void Stop() {
         _listener.Stop();
      }

      public void Start(string location, uint port) {
         _listener.Prefixes.Add(utils.BuildUri(location, port));
         _listener.Start();
         _listener.BeginGetContext(AsyncHandler, _listener);

         utils.PrintListening(Name, location, port);
      }

      private void ResponseHandler(HttpListenerContext context) {
         var found = FindEndpoint(context);

         if (found == null) {
            context.Response.StatusCode = (int) HttpStatusCode.NotFound;
            utils.PrintOutgoing(Name, context, UnregisteredEndoint);
            return;
         }

         context.Response.StatusCode = found.Response.Status;
         context.Response.Headers.Add(found.Response.Headers);
         WriteResponseBody(context, found.Response);
         utils.PrintOutgoing(Name, context);
      }

      private Endpoint FindEndpoint(HttpListenerContext context) {

         var incoming = new Endpoint {
            Request = {
               Url = context.Request.Url.AbsolutePath,
               Method = new List<string> {context.Request.HttpMethod.ToUpper()},
               Headers = CreateNameValueCollection(context.Request.Headers, caseSensitiveKeys: false),
               Query = CreateNameValueCollection(context.Request.QueryString),
               Post = utils.ReadPost(context.Request)
            }
         };

         var found = _endpointDb.Find(incoming);
         return found;
      }

      private static void WriteResponseBody(HttpListenerContext context, Response found) {
         string body;

         try {
            body = File.ReadAllText(found.File);
         } catch {
            body = found.Body;
         }

         if (body != null) utils.WriteBody(context, body);
      }

      private static NameValueCollection CreateNameValueCollection(NameValueCollection collection, bool caseSensitiveKeys = true) {
         var newCollection = new NameValueCollection();

         foreach (var key in collection.AllKeys) {
            newCollection.Add(caseSensitiveKeys ? key : key.ToLower(), collection.Get(key));
         }

         return newCollection;
      }

      private void AsyncHandler(IAsyncResult result) {
         var context = _listener.EndGetContext(result);

         utils.PrintIncoming(Name, context);
         utils.SetServerHeader(context);

         try {
            ResponseHandler(context);
         } catch {
            utils.SetStatus(context, HttpStatusCode.InternalServerError);
            utils.PrintOutgoing(Name, context, UnexpectedError);
         }

         context.Response.Close();
         _listener.BeginGetContext(AsyncHandler, _listener);
      }
   }
}