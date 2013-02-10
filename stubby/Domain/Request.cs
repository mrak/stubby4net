using System.Collections.Generic;

namespace stubby.Domain {

   internal class Request {
      public Request() {
         Headers = new Dictionary<string, string>();
         Query = new Dictionary<string, string>();
         Method = new string[] {};
      }

      public string Url { get; set; }
      public string[] Method { get; set; }
      public IDictionary<string, string> Headers { get; set; }
      public IDictionary<string, string> Query { get; set; }
      public string Post { get; set; }
      public string File { get; set; }
   }

}