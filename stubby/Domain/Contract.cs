using System.Collections.Generic;

namespace stubby.Domain {

   public static class Contract {
      private const string UrlRequired = "request.url is required.";
      private const string RequestRequired = "request is required.";
      private const string UrlSlash = "request.url must begin with '/'.";
      private const string DefaultMethod = "GET";
      public static readonly IList<string> Methods = new[] {"GET", "PUT", "POST", "DELETE", "PATCH", "OPTIONS", "HEAD"};
      public const string MethodInvalid = "request.method must be a valid HTTP verb.";

      public static string[] Verify(Endpoint endpoint) {
         return Verify(new[] {endpoint});
      }

      public static string[] Verify(Endpoint[] endpoints) {
         var errors = new List<string>();

         foreach (var endpoint in endpoints) VerifyRequest(endpoint.Request, errors);
         return errors.ToArray();
      }

      private static void VerifyRequest(Request request, IList<string> errors) {
         if (request == null) {
            errors.Add(RequestRequired);
            return;
         }

         VerifyUrl(request.Url, errors);
         VerifyMethod(request.Method, errors);
      }

      private static void VerifyUrl(string url, IList<string> errors) {
         if (string.IsNullOrWhiteSpace(url)) {
            errors.Add(UrlRequired);
            return;
         }

         if (!url.StartsWith("/"))
            errors.Add(UrlSlash);
      }

      private static void VerifyMethod(string method, IList<string> errors) {
         if (string.IsNullOrWhiteSpace(method)) return;

         if (Methods.Contains(method.ToUpper())) return;

         errors.Add(MethodInvalid);
      }
   }

}