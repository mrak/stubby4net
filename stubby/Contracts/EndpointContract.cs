using System.Collections.Generic;
using System.Linq;
using stubby.Domain;

namespace stubby.Contracts {

   internal static class EndpointContract {
      public static Endpoint Verify(Endpoint endpoint) {
         endpoint = endpoint ?? new Endpoint();

         return new Endpoint {Request = VerifyRequest(endpoint.Request), Response = VerifyResponse(endpoint.Response)};
      }

      private static Request VerifyRequest(Request request) {
         request = request ?? new Request();

         return new Request {
            Url = VerifyUrl(request.Url),
            Query = request.Query ?? new Dictionary<string, string>(),
            Headers = request.Headers ?? new Dictionary<string, string>(),
            Post = request.Post,
            File = request.File,
            Method = VerifyMethod(request.Method)
         };
      }

      private static string VerifyUrl(string url) {
         if (string.IsNullOrWhiteSpace(url)) return "/";
         if (!url.StartsWith("/")) return "/" + url;
         return url;
      }

      private static IList<string> VerifyMethod(ICollection<string> methods) {
         IList<string> verified = new List<string>();

         if (methods == null || methods.Count.Equals(0)) {
            verified.Add("GET");
            return verified;
         }

         foreach (var method in methods.Where(method => !string.IsNullOrWhiteSpace(method)))
            verified.Add(method.ToUpper());

         return verified;
      }

      private static Response VerifyResponse(Response response) {
         response = response ?? new Response();

         return new Response {
            Status = VerifyStatus(response.Status),
            Headers = response.Headers ?? new Dictionary<string, string>(),
            Body = response.Body,
            Latency = response.Latency,
            File = response.File
         };
      }

      private static ushort VerifyStatus(ushort status) {
         if (status < 100 || status >= 600)
            return 200;

         return status;
      }
   }
}