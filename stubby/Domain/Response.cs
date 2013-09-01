using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using Compare = stubby.Domain.ComparisonUtils;

namespace stubby.Domain {

    /// <summary>
    /// Represents the configuration of a stubby response.
    /// </summary>
    [DataContract]
    public class Response {

        public Response() {
            Status = 200;
            Headers = new NameValueCollection();
        }

        /// <summary>
        /// The HTTP/1.1 Status code (100-599).
        /// </summary>
        [DataMember] public ushort Status { get; set; }

        /// <summary>
        /// Name/Value Collection of HTTP Response Headers.
        /// </summary>
        [DataMember] public NameValueCollection Headers { get; set; }

        /// <summary>
        /// The time in milliseconds to wait before responding.
        /// </summary>
        [DataMember] public ulong Latency { get; set; }

        /// <summary>
        /// The content body of the response. If File is supplied and the file exists, this property is ignored.
        /// </summary>
        [DataMember] public string Body { get; set; }

        /// <summary>
        /// A filepath whose file contains the content of the response body. If defined, overrides the Body property.
        /// </summary>
        [DataMember] public string File { get; set; }

        protected bool Equals(Response other) {
            if(Status != other.Status)
                return false;
            if(Latency != other.Latency)
                return false;
            if(!string.Equals(Body, other.Body))
                return false;
            if(!string.Equals(File, other.File))
                return false;
            if(!Compare.NameValueCollections(Headers, other.Headers))
                return false;
            return true;
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = Status.GetHashCode();
                hashCode = (hashCode * 397) ^ (Headers != null ? Headers.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Latency.GetHashCode();
                hashCode = (hashCode * 397) ^ (Body != null ? Body.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (File != null ? File.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override bool Equals(object obj) {
            if(ReferenceEquals(null, obj))
                return false;
            if(ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == GetType() && Equals((Response) obj);
        }
    }
}