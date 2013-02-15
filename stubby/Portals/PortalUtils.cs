using System;
using System.Text;
using stubby.CLI;

namespace stubby.Portals {

   internal static class PortalUtils {
      private const string RequestResponseFormat = "[{0}] {1} {2} [{3}]{4}";
      private const string IncomingArrow = "-->";
      private const string OutgoingArrow = "<--";

      public static void PrintIncoming(string portal, string url, string method) {
         Out.Incoming(string.Format(RequestResponseFormat, DateTime.Now.ToString("T"), IncomingArrow, method, portal,
                                    url));
      }

      public static void PrintOutgoing(string portal, string url, int status) {
         var now = DateTime.Now.ToString("T");
         Action<string> function;

         if (status < 200) function = Out.Info;
         else if (status < 300) function = Out.Success;
         else if (status < 400) function = Out.Notice;
         else if (status < 500) function = Out.Warn;
         else function = Out.Error;

         function.Invoke(String.Format(RequestResponseFormat, now, OutgoingArrow, status, portal, url));
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