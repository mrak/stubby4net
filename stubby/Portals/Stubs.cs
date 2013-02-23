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
      private const string UnregisteredEndoint = "is not a registered endpoint.";
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
         utils.PrintIncoming(Name, context);
         utils.SetServerHeader(context);

         var found = FindEndpoint(context);

         if (found == null) {
            context.Response.StatusCode = (int) HttpStatusCode.NotFound;
            context.Response.Close();
            utils.PrintOutgoing(Name, context, UnregisteredEndoint);
            return;
         }

         context.Response.StatusCode = found.Response.Status;
         context.Response.Headers = CreateWebHeaderCollection(found.Response.Headers);
         WriteResponseBody(context.Response, found.Response);
         utils.PrintOutgoing(Name, context);
      }

      private Endpoint FindEndpoint(HttpListenerContext context) {

         var incoming = new Endpoint {
            Request = {
               Url = context.Request.Url.AbsolutePath,
               Method = new List<string> {context.Request.HttpMethod},
               Headers = CreateDictionary(context.Request.Headers),
               Query = CreateDictionary(context.Request.QueryString),
               Post = utils.ReadPost(context.Request)
            }
         };

         var found = _endpointDb.Find(incoming);
         return found;
      }

      private static void WriteResponseBody(HttpListenerResponse response, Response found) {
         string body;

         try {
            body = File.ReadAllText(found.File);
         } catch {
            body = found.Body;
         }

         if (body == null) response.Close();

         else {
            var bytes = new byte[body.Length*sizeof (char)];
            Buffer.BlockCopy(body.ToCharArray(), 0, bytes, 0, bytes.Length);
            response.Close(bytes, false);
         }
      }

      private static IDictionary<string, string> CreateDictionary(NameValueCollection collection) {
         return collection.AllKeys.ToDictionary(key => key, collection.Get);
      }

      private static WebHeaderCollection CreateWebHeaderCollection(IDictionary<string, string> headers) {
         var collection = new WebHeaderCollection();

         foreach (var header in headers) collection.Add(header.Key, header.Value);

         return collection;
      }

      private void AsyncHandler(IAsyncResult result) {
         var context = _listener.EndGetContext(result);
         ResponseHandler(context);
         _listener.BeginGetContext(AsyncHandler, _listener);
      }
   }
}