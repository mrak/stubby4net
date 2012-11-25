using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace stubby.Portals {

   public class Portal {
      private readonly HttpListener _listener;

      public Portal() : this(new HttpListener()) {}

      public Portal(HttpListener listener) {
         _listener = listener;
      }

      public void Start(string location, uint port) {
         _listener.Prefixes.Add(BuildUri(location, port));
         _listener.Start();
      }

      public string BuildUri(string location, uint port) {
         var stringBuilder = new StringBuilder("");
         
         stringBuilder.Append("http://");
         stringBuilder.Append(location);
         stringBuilder.Append(":");
         stringBuilder.Append(port);
         stringBuilder.Append("/");

         return stringBuilder.ToString();
      }

      public async void RespondWith(WaitCallback handler) {
         try {
            var context = await _listener.GetContextAsync();
            ThreadPool.QueueUserWorkItem(handler, context);
         } catch (Exception ex) {
            Console.WriteLine(ex.Message);
            //log
         }
      }
   }

}