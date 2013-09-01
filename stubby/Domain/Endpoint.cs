using System.Runtime.Serialization;

namespace stubby.Domain {

    /// <summary>
    /// A representation of a Request/Response pairing used by stubby.
    /// </summary>
    [DataContract]
    public class Endpoint {

        public Endpoint() {
            Request = new Request();
            Response = new Response();
        }

        /// <summary>
        /// Used to match against incoming requests.
        /// </summary>
        [DataMember]
        public Request Request { get; set; }

        /// <summary>
        /// Used to generate a response to the client.
        /// </summary>
        [DataMember]
        public Response Response { get; set; }

        public override int GetHashCode() {
            unchecked {
                return ((Request != null ? Request.GetHashCode() : 0) * 397) ^ (Response != null ? Response.GetHashCode() : 0);
            }
        }

        public override bool Equals(object obj) {
            if(ReferenceEquals(null, obj))
                return false;
            if(ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == GetType() && Equals((Endpoint) obj);
        }

        protected bool Equals(Endpoint other) {
            return Equals(Request, other.Request) && Equals(Response, other.Response);
        }

        internal bool Matches(Endpoint other) {
            return Request.Matches(other.Request);
        }
    }
}