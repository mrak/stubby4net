using System.Collections.Generic;

namespace stubby.Domain {

   internal class Request {
      public Request() {
         Headers = new Dictionary<string, string>();
         Query = new Dictionary<string, string>();
         Method = new List<string> {"GET"};
      }

      public string Url { get; set; }
      public List<string> Method { get; set; }
      public IDictionary<string, string> Headers { get; set; }
      public IDictionary<string, string> Query { get; set; }
      public string Post { get; set; }
      public string File { get; set; }

      public override bool Equals(object o) {
         var other = (Request) o;
         if (Url != other.Url) return false;
         if (!Method.Contains(other.Method[0])) return false;

         foreach (var header in Headers) {
            if (!other.Headers.ContainsKey(header.Key)) return false;
            if (other.Headers[header.Key] != header.Value) return false;
         }
         foreach (var query in Query) {
            if (!other.Query.ContainsKey(query.Key)) return false;
            if (other.Query[query.Key] != query.Value) return false;
         }

         try {
            return System.IO.File.ReadAllText(File) == other.Post;
         } catch {
            if (Post != null && Post != other.Post) return false;            
         }

         return true;
      }
   }

}