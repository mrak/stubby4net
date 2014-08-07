using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using NUnit.Framework;
using stubby;
using stubby.Domain;

namespace integration
{
	[TestFixture]
	public class InvocationsTest
	{
		private readonly Stubby _stubby = new Stubby(new Arguments
		{
			Admin = 9999,
			Stubs = 9992,
			Mute = true,
			Data = "../../YAML/invocations.yaml"
		});

		[TestFixtureSetUp]
		public void Before()
		{
			_stubby.Start();
		}

		[TestFixtureTearDown]
		public void After()
		{
			_stubby.Stop();
		}

		[Test]
		public void BasicGETWithTenantId_ShouldReturnOK()
		{
			var tenantId = Guid.NewGuid().ToString();
			WebRequest.Create("http://localhost:9992/basic")
				.AddHeader("tenantId", tenantId)
				.Get();

			WebRequest.Create("http://localhost:9992/basic")
				.AddHeader("tenantId", tenantId)
				.Get();

			var invocations = WebRequest.Create("http://localhost:9999/invocations/basic")
				.AddIgnoreHeaders()
				.AddHeader("tenantId", tenantId)
				.InvocationsGet();

			Assert.That(invocations.Count, Is.EqualTo(2));
			foreach (var invocation in invocations)
			{
				Assert.That(invocation.Headers["tenantid"][0], Is.EqualTo(tenantId));
			}
		}

		[Test]
		public void BasicGETWithQueryString_ShouldReturnOK()
		{
			const string postData = "This is a test that posts this string to a Web server.";
			var tenantId = Guid.NewGuid().ToString();
			var value1 = Guid.NewGuid().ToString();
			var value2 = Guid.NewGuid().ToString();
			WebRequest.Create(String.Format("http://localhost:9992/basic?key1={0}&key2={1}", value1, value2))
				.AddHeader("tenantId", tenantId)
				.Post(postData);

			WebRequest.Create(String.Format("http://localhost:9992/basic?key1={0}&key2={1}", value1, value2))
				.AddHeader("tenantId", tenantId + "something-else")
				.Get();

			WebRequest.Create("http://localhost:9992/basic?key1=value1&key2=value2")
				.AddHeader("tenantId", tenantId)
				.Get();

			var invocations = WebRequest.Create(String.Format("http://localhost:9999/invocations/basic?key2={0}", value2))
				.AddHeader("tenantId", tenantId)
				.AddIgnoreHeaders()
				.InvocationsPost();

			Assert.That(invocations.Count, Is.EqualTo(1));
			var invocation = invocations[0];
			Assert.That(invocation.Method, Is.EqualTo("POST"));
			Assert.That(invocation.Url, Is.EqualTo("/basic"));
			Assert.That(invocation.Headers.Keys, Contains.Item("tenantid"));
			Assert.That(invocation.Headers["tenantid"][0], Is.EqualTo(tenantId));
			Assert.That(invocation.Post, Is.EqualTo(postData));
		}

		[Test]
		public void BasicGET_POST_PUT_DELETE_ShouldReturnFilter()
		{
			var requestId = Guid.NewGuid();
			WebRequest.Create("http://localhost:9992/basic")
				.AddHeader("requestId", requestId.ToString())
				.Delete();

			WebRequest.Create("http://localhost:9992/basic")
				.AddHeader("requestId", requestId.ToString())
				.Get();

			WebRequest.Create("http://localhost:9992/basic")
				.AddHeader("requestId", requestId.ToString())
				.Post();

			WebRequest.Create("http://localhost:9992/basic")
				.AddHeader("requestId", requestId.ToString())
				.Put();

			var invocations = WebRequest.Create("http://localhost:9999/invocations/basic")
				.AddIgnoreHeaders()
				.AddHeader("requestId", requestId.ToString())
				.InvocationsDelete();

			Assert.That(invocations.Count, Is.EqualTo(1));
			var invocation = invocations[0];
			Assert.That(invocation.Method, Is.EqualTo("DELETE"));
			Assert.That(invocation.Url, Is.EqualTo("/basic"));
			Assert.That(invocation.Headers.Keys, Contains.Item("requestid"));
			Assert.That(invocation.Headers["requestid"][0], Is.EqualTo(requestId.ToString()));

			invocations = WebRequest.Create("http://localhost:9999/invocations/basic")
				.AddIgnoreHeaders()
				.AddHeader("requestId", requestId.ToString())
				.InvocationsPost();

			Assert.That(invocations.Count, Is.EqualTo(1));
			invocation = invocations[0];
			Assert.That(invocation.Method, Is.EqualTo("POST"));
			Assert.That(invocation.Url, Is.EqualTo("/basic"));
			Assert.That(invocation.Headers.Keys, Contains.Item("requestid"));
			Assert.That(invocation.Headers["requestid"][0], Is.EqualTo(requestId.ToString()));

			invocations = WebRequest.Create("http://localhost:9999/invocations/basic")
				.AddIgnoreHeaders()
				.AddHeader("requestId", requestId.ToString())
				.InvocationsPut();

			Assert.That(invocations.Count, Is.EqualTo(1));
			invocation = invocations[0];
			Assert.That(invocation.Method, Is.EqualTo("PUT"));
			Assert.That(invocation.Url, Is.EqualTo("/basic"));
			Assert.That(invocation.Headers.Keys, Contains.Item("requestid"));
			Assert.That(invocation.Headers["requestid"][0], Is.EqualTo(requestId.ToString()));

			invocations = WebRequest.Create("http://localhost:9999/invocations/basic")
				.AddIgnoreHeaders()
				.AddHeader("requestId", requestId.ToString())
				.InvocationsGet();

			Assert.That(invocations.Count, Is.EqualTo(1));
			invocation = invocations[0];
			Assert.That(invocation.Method, Is.EqualTo("GET"));
			Assert.That(invocation.Url, Is.EqualTo("/basic"));
			Assert.That(invocation.Headers.Keys, Contains.Item("requestid"));
			Assert.That(invocation.Headers["requestid"][0], Is.EqualTo(requestId.ToString()));
		}
	}

	public static class TestExtensions
	{
		private static HttpWebResponse GetResponse(WebRequest request, string method, string body)
		{
			request.Method = method;
			request.SetBody(body);
			using (var response = (HttpWebResponse)request.GetResponse())
			{
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
				response.Close();
				return response;
			}
		}

		public static WebRequest AddIgnoreHeaders(this WebRequest request)
		{
			request.Headers.Add("x-ignore", "host");
			request.Headers.Add("x-ignore", "connection");
			request.Headers.Add("x-ignore", "content-length");
			return request;
		}

		public static WebRequest AddHeader(this WebRequest request, string name, string value)
		{
			request.Headers.Add(name, value);
			return request;
		}

		public static HttpWebResponse Post(this WebRequest request, string body = null)
		{
			return GetResponse(request, "POST", body);
		}

		public static HttpWebResponse Put(this WebRequest request, string body = null)
		{
			return GetResponse(request, "PUT", body);
		}

		public static HttpWebResponse Get(this WebRequest request)
		{
			return GetResponse(request, "GET", null);
		}

		public static HttpWebResponse Delete(this WebRequest request)
		{
			return GetResponse(request, "DELETE", null);
		}

		public static IList<Invocation> InvocationsPost(this WebRequest request, string body = null)
		{
			return LoadInvocations(request, "POST", body);
		}

		public static IList<Invocation> InvocationsGet(this WebRequest request, string body = null)
		{
			return LoadInvocations(request, "GET", body);
		}

		public static IList<Invocation> InvocationsPut(this WebRequest request, string body = null)
		{
			return LoadInvocations(request, "PUT", body);
		}

		public static IList<Invocation> InvocationsDelete(this WebRequest request, string body = null)
		{
			return LoadInvocations(request, "DELETE", body);
		}

		private static IList<Invocation> LoadInvocations(WebRequest request, string method, string body)
		{
			request.Method = method;
			request.SetBody(body);
			using (var response = (HttpWebResponse)request.GetResponse())
			{
				Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
				var responseStream = response.GetResponseStream();
				if (responseStream == null)
					return new List<Invocation>();

				using (var reader = new StreamReader(responseStream))
				{
					var content = reader.ReadToEnd();
					var serializer = new JavaScriptSerializer();
					var invocations = serializer.Deserialize<List<Invocation>>(content);
					return invocations;
				}
			}
		}

		private static WebRequest SetBody(this WebRequest request, string body = null)
		{
			switch (request.Method.ToLowerInvariant())
			{
				case "post":
				case "put":
					if (String.IsNullOrEmpty(body))
					{
						request.ContentLength = 0;
					}
					else
					{
						var byteArray = Encoding.UTF8.GetBytes(body);
						request.ContentLength = byteArray.Length;
						var dataStream = request.GetRequestStream();
						dataStream.Write(byteArray, 0, byteArray.Length);
						dataStream.Close();
					}
					break;
			}
			return request;
		}
	}
}
