using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Compare = stubby.Domain.ComparisonUtils;

namespace stubby.Domain {

    /// <summary>
    /// Represents the data to match incoming requests against.
    /// </summary>
    [DataContract]
    public class Request {

        public Request() {
            Method = new List<string> { "GET" };
            Query = new NameValueCollection();
            Headers = new NameValueCollection();
        }

        /// <summary>
        /// A regex string to match incoming URI paths to. Incoming query parameters are ignored.
        /// </summary>
        [DataMember] public string Url { get; set; }

        /// <summary>
        /// A list of acceptable HTTP verbs such as GET or POST. Defaults to GET.
        /// </summary>
        [DataMember] public IList<string> Method { get; set; }

        /// <summary>
        /// Name/Value headers that incoming requests must at least possess.
        /// </summary>
        [DataMember] public NameValueCollection Headers { get; set; }

        /// <summary>
        /// A Name/Value collection of URI Query parameters that must be present.
        /// </summary>
        [DataMember] public NameValueCollection Query { get; set; }

        /// <summary>
        /// The post body contents of the incoming request. If File is specified and exists upon request, this value is ignored.
        /// </summary>
        [DataMember] public string Post { get; set; }

        /// <summary>
        /// A filepath whose file contains the incoming request body to match against. If the file cannot be found, Post is used instead.
        /// </summary>
        [DataMember] public string File { get; set; }

        internal bool Matches(Request other) {
            if(!Regex.IsMatch(other.Url, Url))
                return false;
            if(!Method.Contains(other.Method[0]))
                return false;

            foreach(var header in Headers.AllKeys) {
                IList<string> otherValues = other.Headers.GetValues(header);
                if(otherValues == null)
                    return false;
                if(!Headers.GetValues(header).All(h => otherValues.Any(o => Regex.IsMatch(o, h))))
                    return false;
            }

            foreach(var variable in Query.AllKeys) {
                IList<string> otherValues = other.Query.GetValues(variable);
                if(otherValues == null)
                    return false;
                if(!Query.GetValues(variable).All(q => otherValues.Any(o => Regex.IsMatch(o, q))))
                    return false;
            }

            try {
                return Regex.IsMatch(other.Post, System.IO.File.ReadAllText(File).TrimEnd(' ', '\n', '\r', '\t'));
            } catch {
                if(Post != null && !Regex.IsMatch(other.Post, Post))
                    return false;
            }

            return true;
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = (Url != null ? Url.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Method != null ? Method.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Headers != null ? Headers.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Query != null ? Query.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Post != null ? Post.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (File != null ? File.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override bool Equals(object obj) {
            if(ReferenceEquals(null, obj))
                return false;
            if(ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == GetType() && Equals((Request) obj);
        }

        protected bool Equals(Request other) {
            if(!string.Equals(Url, other.Url))
                return false;
            if(!string.Equals(Post, other.Post))
                return false;
            if(!string.Equals(File, other.File))
                return false;
            if(!Compare.Lists(Method, other.Method))
                return false;
            if(!Compare.NameValueCollections(Headers, other.Headers))
                return false;
            if(!Compare.NameValueCollections(Query, other.Query))
                return false;
            return true;
        }
    }
}