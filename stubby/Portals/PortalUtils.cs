﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using stubby.CLI;
using System.Web.Script.Serialization;
using stubby.Domain;

namespace stubby.Portals {

	internal static class PortalUtils
	{
		private const string RequestResponseFormat = "[{0}] {1} {2} [{3}]{4} {5}";
		private const string ListeningString = "{0} portal listening at http{3}://{1}:{2}";
		private const string IncomingArrow = "->";
		private const string OutgoingArrow = "<-";
		private const string JsonMimeType = "application/json";
		private const string HtmlMimeType = "text/html";
		private static readonly string ServerHeader = "stubby4net/" + Stubby.Version;

		public static void PrintIncoming(string portal, HttpListenerContext context, string message = "")
		{
			var url = context.Request.Url.AbsolutePath;
			var method = context.Request.HttpMethod;

			Out.Incoming(String.Format(RequestResponseFormat,
			                           DateTime.Now.ToString("T"), IncomingArrow, method, portal, url, message));
		}

		public static void PrintOutgoing(string portal, HttpListenerContext context, string message = "")
		{
			var url = context.Request.Url.AbsolutePath;
			var status = context.Response.StatusCode;
			var now = DateTime.Now.ToString("T");
			Action<string> function;

			if (status < 200)
				function = Out.Info;
			else if (status < 300)
				function = Out.Success;
			else if (status < 400)
				function = Out.Warn;
			else
				function = Out.Error;

			function.Invoke(String.Format(RequestResponseFormat, now, OutgoingArrow, status, portal, url, message));
		}

		public static void SetServerHeader(HttpListenerContext context)
		{
			context.Response.AddHeader("Server", ServerHeader);
		}

		public static void SetJsonType(HttpListenerContext context)
		{
			context.Response.Headers.Set(HttpResponseHeader.ContentType, JsonMimeType);
		}

		public static void SetHtmlType(HttpListenerContext context)
		{
			context.Response.Headers.Set(HttpResponseHeader.ContentType, HtmlMimeType);
		}

		public static void SerializeToJson<T>(T entity, HttpListenerContext context)
		{
			var serializer = new JavaScriptSerializer();
			SetJsonType(context);
			WriteBody(context, serializer.Serialize(entity));
		}

		public static void WriteBody(HttpListenerContext context, string body)
		{
			using (var writer = new StreamWriter(context.Response.OutputStream))
			{
				writer.Write(body);
			}
		}

		public static string ReadPost(HttpListenerRequest request)
		{
			if (request.ContentLength64.Equals(0))
				return null;
			using (var reader = new StreamReader(request.InputStream))
			{
				return reader.ReadToEnd();
			}
		}

		public static string BuildUri(string location, uint port, bool https = false)
		{
			var stringBuilder = new StringBuilder("");

			stringBuilder.Append(https ? "https://" : "http://");
			stringBuilder.Append(location);
			stringBuilder.Append(":");
			stringBuilder.Append(port);
			stringBuilder.Append("/");

			return stringBuilder.ToString();
		}

		public static byte[] GetBytes(string str)
		{
			var bytes = new byte[str.Length * sizeof (char)];
			Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		public static void PrintListening(string name, string location, uint port, bool https = false)
		{
			Out.Status(String.Format(ListeningString, name, location, port, https ? "s" : String.Empty));
		}

		public static void SetStatus(HttpListenerContext context, int status)
		{
			context.Response.StatusCode = status;
		}

		public static void SetStatus(HttpListenerContext context, HttpStatusCode status)
		{
			SetStatus(context, (int) status);
		}

		public static void AddLocationHeader(HttpListenerContext context, uint id)
		{
			context.Response.Headers.Add(HttpResponseHeader.Location, "/" + id);
		}
	}

	public static class Extensions
	{
		internal static Invocation ToInvocation(this HttpListenerContext context)
		{
			return context.ToEndpoint().ToInvocation();
		}

		internal static Invocation ToInvocation(this Endpoint endpoint)
		{
			return new Invocation
			{
				Url = endpoint.Request.Url,
				Method = endpoint.Request.Method[0],
				Headers = ToDictionary(endpoint.Request.Headers),
				Query = ToDictionary(endpoint.Request.Query),
				Post = endpoint.Request.Post
			};
		}

		internal static Endpoint ToEndpoint(this HttpListenerContext context)
		{
			return new Endpoint
			{
				Request =
				{
					Url = context.Request.Url.AbsolutePath,
					Method = new List<string> { context.Request.HttpMethod.ToUpper() },
					Headers = CreateNameValueCollection(context.Request.Headers, caseSensitiveKeys: false),
					Query = CreateNameValueCollection(context.Request.QueryString, caseSensitiveKeys: true),
					Post = PortalUtils.ReadPost(context.Request)
				}
			};
		}

		private static IDictionary<string, IList<string>> ToDictionary(NameValueCollection nvc)
		{
			var result = new Dictionary<string, IList<string>>();
			foreach (string key in nvc.Keys)
			{
				var values = nvc.GetValues(key);
				result.Add(key.ToLowerInvariant(), values == null ? new List<string>() : new List<string>(values));
			}
			return result;
		}

		private static NameValueCollection CreateNameValueCollection(NameValueCollection collection, bool caseSensitiveKeys)
		{
			var newCollection = new NameValueCollection();
			foreach (var key in collection.AllKeys)
			{
				newCollection.Add(caseSensitiveKeys ? key : key.ToLower(), collection.Get(key));
			}
			return newCollection;
		}
	}
}
