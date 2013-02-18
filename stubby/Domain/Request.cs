using System.Collections.Generic;
using System.Runtime.Serialization;

namespace stubby.Domain {

   [DataContract]
   public class Request {
      private IDictionary<string, string> _headers = new Dictionary<string, string>();
      private IList<string> _method = new List<string>();
      private IDictionary<string, string> _query = new Dictionary<string, string>();
      private string _url;

      [DataMember]
      public string Url {
         get { return _url; }
         set {
            if (value == null) _url = "/";
            else if (!value.StartsWith("/")) _url = "/" + value;
            else _url = value;
         }
      }

      [DataMember]
      public IList<string> Method {
         get { return _method ?? (_method = new List<string> {"GET"}); }
         set { _method = value; }
      }

      [DataMember]
      public IDictionary<string, string> Headers {
         get { return _headers ?? (_headers = new Dictionary<string, string>()); }
         set { _headers = value; }
      }

      [DataMember]
      public IDictionary<string, string> Query {
         get { return _query ?? (_query = new Dictionary<string, string>()); }
         set { _query = value; }
      }

      [DataMember]
      public string Post { get; set; }

      [DataMember]
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