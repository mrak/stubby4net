using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using stubby.CLI;

namespace stubby.Portals {

   internal static class PortalUtils {
      private const string RequestResponseFormat = "[{0}] {1} {2} [{3}]{4} {5}";
      private const string IncomingArrow = "->";
      private const string OutgoingArrow = "<-";
      private static readonly string Version =
         FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;

      public static void PrintIncoming(string portal, string url, string method, string message = "") {
         Out.Incoming(string.Format(RequestResponseFormat, DateTime.Now.ToString("T"), IncomingArrow, method, portal,
                                    url, message));
      }

      public static void PrintOutgoing(string portal, string url, int status, string message = "") {
         var now = DateTime.Now.ToString("T");
         Action<string> function;

         if (status < 200) function = Out.Info;
         else if (status < 300) function = Out.Success;
         else if (status < 400) function = Out.Warn;
         else function = Out.Error;

         function.Invoke(String.Format(RequestResponseFormat, now, OutgoingArrow, status, portal, url, message));
      }

      public static void AddServerHeader(HttpListenerResponse response) {
         response.AddHeader("Server", "stubby4net/" + Version);
      }

      public static string BuildUri(string location, uint port) {
         var stringBuilder = new StringBuilder("");

         stringBuilder.Append("http://");
         stringBuilder.Append(location);
         stringBuilder.Append(":");
         stringBuilder.Append(port);
         stringBuilder.Append("/");

         return stringBuilder.ToString();
      }
   }

}