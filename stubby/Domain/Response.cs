using System.Collections.Generic;

namespace stubby.Domain {

   public class Response {
      public Response() {
         Headers = new Dictionary<string, string>();
      }

      public ushort Status { get; set; }
      public IDictionary<string, string> Headers { get; set; }
      public ulong Latency { get; set; }
      public string Body { get; set; }
      public string File { get; set; }
   }

}