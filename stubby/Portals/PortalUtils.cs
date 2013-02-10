using System.Text;

namespace stubby.Portals
{
   internal static class PortalUtils
   {
      public static string BuildUri(string location, uint port)
      {
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
