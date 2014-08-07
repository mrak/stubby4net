using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using stubby.Portals;
using System.Linq;

namespace stubby.Domain
{
	[DataContract]
	public class Invocation
	{
		/// <summary>
		/// A regex string to match incoming URI paths to. Incoming query parameters are ignored.
		/// </summary>
		[DataMember]
		public string Url { get; set; }

		/// <summary>
		/// A list of acceptable HTTP verbs such as GET or POST.
		/// </summary>
		[DataMember]
		public string Method { get; set; }

		/// <summary>
		/// Name/Value headers that incoming requests must at least possess.
		/// </summary>
		[DataMember]
		public IDictionary<string, IList<string>> Headers { get; set; }

		/// <summary>
		/// A Name/Value collection of URI Query parameters that must be present.
		/// </summary>
		[DataMember]
		public IDictionary<string, IList<string>> Query { get; set; }

		/// <summary>
		/// The post body contents of the incoming request. If File is specified and exists upon request, this value is ignored.
		/// </summary>
		[DataMember]
		public string Post { get; set; }

		public override string ToString()
		{
			var query = ToString(Query, "&");
			var headers = ToString(Headers, ",");
			return string.Format("{0}: {1}{2}{3} Headers: {4}, Body: {5}",
			                     Method,
			                     Url,
			                     string.IsNullOrEmpty(query) ? string.Empty : "?",
								 query,
			                     headers,
			                     Post);
		}

		private static string ToString(IDictionary<string, IList<string>> dict, string separator)
		{
			var str = new StringBuilder();
			foreach (var entry in dict)
			{
				var valueString = string.Join(",", entry.Value);
				str.AppendFormat("{0}={1}{2}", entry.Key, valueString, separator);
			}
			if (str.Length > 0)
				str.Length -= separator.Length;

			return str.ToString();
		}
	}
}
