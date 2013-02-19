using System.Collections.Generic;
using System.Runtime.Serialization;

namespace stubby.Domain {

   [DataContract]
   public class Response {
      public Response() {
         Status = 200;
         Headers = new Dictionary<string, string>();
      }

      [DataMember]
      public ushort Status { get; set; }

      [DataMember]
      public IDictionary<string, string> Headers { get; set; }

      [DataMember]
      public ulong Latency { get; set; }

      [DataMember]
      public string Body { get; set; }

      [DataMember]
      public string File { get; set; }
   }

}