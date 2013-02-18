using System.Collections.Generic;
using System.Runtime.Serialization;

namespace stubby.Domain {

   [DataContract]
   public class Response {
      private IDictionary<string, string> _headers = new Dictionary<string, string>();
      private ushort _status = 200;

      [DataMember]
      public ushort Status {
         get { return _status; }
         set {
            if (value < 100 || value > 599) _status = 200;
            else _status = value;
         }
      }

      [DataMember]
      public IDictionary<string, string> Headers {
         get { return _headers ?? (_headers = new Dictionary<string, string>()); }
         set { _headers = value; }
      }

      [DataMember]
      public ulong Latency { get; set; }

      [DataMember]
      public string Body { get; set; }

      [DataMember]
      public string File { get; set; }
   }

}