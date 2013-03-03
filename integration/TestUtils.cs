using System;
using System.IO;
using System.Net;
using System.Text;

namespace integration
{
   internal static class TestUtils
   {
      public static string EncodeBasicAuth(string username, string password) {
         var base64Auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));
         return "Basic " + base64Auth;
      }
      public static void WritePost(WebRequest request, string post) {
         using(var stream = request.GetRequestStream())
         using (var writer = new StreamWriter(stream)) {
            writer.Write(post);
         }
      }

      public static string ExtractBody(HttpWebResponse response) {
         Encoding encoding;

         try {
            encoding = Encoding.GetEncoding(response.CharacterSet);
         } catch (ArgumentException) {
            encoding = Encoding.Default;
         }

         using(var responseStream = response.GetResponseStream())
         using (var reader = new StreamReader(responseStream, encoding)) {
            return reader.ReadToEnd();
         }
      }
   }
}
