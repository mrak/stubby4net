using System.Collections.Generic;
using stubby.Domain;

namespace stubby.Contracts {

   public static class EndpointContract {
      private const string UrlRequired = "request.url is required.";
      private const string RequestRequired = "request is required.";
      private const string UrlSlash = "request.url must begin with '/'.";
      private const string MethodInvalid = "request.method \"{0}\" is not an accepted HTTP verb.";
      private const string StatusInvalid = "response.status must be between 100 and 599.";

      private static readonly ICollection<string> Methods = new[]
      {"GET", "PUT", "POST", "DELETE", "PATCH", "OPTIONS", "HEAD"};

      public static string[] Verify(Endpoint endpoint) {
         return Verify(new[] {endpoint});
      }

      public static string[] Verify(Endpoint[] endpoints) {
         var errors = new List<string>();

         foreach (var endpoint in endpoints) {
            VerifyRequest(endpoint.Request, errors);
            VerifyResponse(endpoint.Response, errors);
         }
         return errors.ToArray();
      }

      private static void VerifyResponse(Response response, ICollection<string> errors) {
         if (response == null) return;

         VerifyStatus(response.Status, errors);
      }

      private static void VerifyStatus(ushort status, ICollection<string> errors) {
         if (status == 0) return;

         if (status < 100 || status >= 600)
            errors.Add(StatusInvalid);
      }

      private static void VerifyRequest(Request request, ICollection<string> errors) {
         if (request == null) {
            errors.Add(RequestRequired);
            return;
         }

         VerifyUrl(request.Url, errors);
         VerifyMethod(request.Method, errors);
      }

      private static void VerifyUrl(string url, ICollection<string> errors) {
         if (string.IsNullOrWhiteSpace(url)) {
            errors.Add(UrlRequired);
            return;
         }

         if (!url.StartsWith("/"))
            errors.Add(UrlSlash);
      }

      private static void VerifyMethod(IEnumerable<string> methods, ICollection<string> errors) {
         foreach (var method in methods) {
            if (string.IsNullOrWhiteSpace(method)) continue;
            if (Methods.Contains(method.ToUpper())) continue;

            errors.Add(string.Format(MethodInvalid, method));
         }
      }
   }

}