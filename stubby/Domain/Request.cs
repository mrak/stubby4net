using System.Collections.Generic;

namespace stubby.Domain {

   public class Request {
      public Request() {
         Headers = new Dictionary<string, string>();
         Query = new Dictionary<string, string>();
      }

      public string Url { get; set; }
      public string Method { get; set; }
      public IDictionary<string, string> Headers { get; set; }
      public IDictionary<string, string> Query { get; set; }
      public string Post { get; set; }
      public string File { get; set; }
   }

}