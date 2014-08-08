using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using stubby.CLI;
using stubby.Domain;

namespace unit
{
	[TestFixture]
	class InvocationDbTest
	{
		private InvocationDb _invocationDb;

		[SetUp]
		public void BeforeEach()
		{
			Out.Mute = true;
			_invocationDb = new InvocationDb();
		}

		[Test]
		public void Add_ShouldIndexByMethodAndUrl()
		{
			var random = new Random();
			var verbs = new[] {"GET", "PUT", "POST", "DELETE", "HEAD"};

			var invocations = new List<Invocation>();
			for (var i = 0; i < verbs.Length; i++)
			{
				var verb = verbs[i];
				var invocationCount = random.Next(5, 100);
				for (var j = 0; j < invocationCount; j++)
				{
					var resourceName = "/resource" + random.Next(1, 10);
					invocations.Add(new Invocation {Method = verb, Url = resourceName});
				}
			}

			invocations.ForEach(i => _invocationDb.Add(i));

			var invocationDict = invocations.GroupBy(x => x.Method)
				.ToDictionary(k1 => k1.Key, v1 => v1.GroupBy(x => x.Url).ToDictionary(k2 => k2.Key, v2 => v2.ToList()));

			foreach (var methodEntry in invocationDict)
			{
				var method = methodEntry.Key;
				foreach (var urlEntry in methodEntry.Value)
				{
					var url = urlEntry.Key;
					var expectedInvocations = urlEntry.Value;
					var actualInvocations = _invocationDb.Find(new Invocation {Method = method, Url = url});
					Assert.That(expectedInvocations.Count, Is.EqualTo(actualInvocations.Count));
				}
			}
		}

		[Test]
		public void Find_ShouldReturnEmptyList_WhenEmpty()
		{
			var invocations = _invocationDb.Find(new Invocation {Method = "GET"});
			Assert.That(invocations.Count, Is.EqualTo(0));
		}

		[Test]
		public void Find_ShouldReturnEmptyList_WhenEmptyUrls()
		{
			_invocationDb.Add(new Invocation { Method = "GET", Url = "/url1"});
			var invocations = _invocationDb.Find(new Invocation { Method = "GET", Url = "/url2" });
			Assert.That(invocations.Count, Is.EqualTo(0));
		}

		[Test]
		public void Find_ShouldNotMatch_WhenIncomingHeaderIsNull()
		{
			_invocationDb.Add(new Invocation {Method = "GET", Url = "/url1", Headers = new Dictionary<string, IList<string>>()});
			var invocations = _invocationDb.Find(new Invocation { Method = "GET", Url = "/url1" });
			Assert.That(invocations.Count, Is.EqualTo(0));
		}

		[Test]
		public void Find_ShouldNotMatch_WhenInvocationHeaderIsNull()
		{
			_invocationDb.Add(new Invocation { Method = "GET", Url = "/url1"});
			var invocations = _invocationDb.Find(new Invocation {Method = "GET", Url = "/url1", Headers = new Dictionary<string, IList<string>>()});
			Assert.That(invocations.Count, Is.EqualTo(0));
		}

		[Test]
		public void Find_ShouldNotMatch_WhenInvocationQueryIsNull()
		{
			_invocationDb.Add(new Invocation { Method = "GET", Url = "/url1" });
			var invocations = _invocationDb.Find(new Invocation { Method = "GET", Url = "/url1", Query = new Dictionary<string, IList<string>>() });
			Assert.That(invocations.Count, Is.EqualTo(0));
		}

		[Test]
		public void Find_ShouldNotMatch_WhenInvocationHeadersDoNotHaveIncomingHeaderKey()
		{
			var invocationHeader = new Dictionary<string, IList<string>> {{"header1", new List<string>()}};
			var incomingHeader = new Dictionary<string, IList<string>> {{"header2", new List<string>()}};

			_invocationDb.Add(new Invocation { Method = "GET", Url = "/url1", Headers = invocationHeader });
			var invocations = _invocationDb.Find(new Invocation {Method = "GET", Url = "/url1", Headers = incomingHeader});
			Assert.That(invocations.Count, Is.EqualTo(0));
		}

		[Test]
		public void Find_ShouldMatch_WhenInvocationHeaderExistsButIncomingHeaderHasNoValues()
		{
			var invocationHeader = new Dictionary<string, IList<string>> {{"header1", new List<string> {"a", "b", "c"}}};
			var incomingHeader = new Dictionary<string, IList<string>> {{"header1", new List<string>()}};

			_invocationDb.Add(new Invocation { Method = "GET", Url = "/url1", Headers = invocationHeader });
			var invocations = _invocationDb.Find(new Invocation { Method = "GET", Url = "/url1", Headers = incomingHeader });
			Assert.That(invocations.Count, Is.EqualTo(1));
		}

		[Test]
		public void Find_ShouldNotMatch_WhenInvocationHeadersDoNotHaveIncomingHeaderValue()
		{
			var invocationHeader = new Dictionary<string, IList<string>> { { "header1", new List<string> { "a", "b", "c" } } };
			var incomingHeader = new Dictionary<string, IList<string>> { { "header1", new List<string> { "d" } } };

			_invocationDb.Add(new Invocation { Method = "GET", Url = "/url1", Headers = invocationHeader });
			var invocations = _invocationDb.Find(new Invocation { Method = "GET", Url = "/url1", Headers = incomingHeader });
			Assert.That(invocations.Count, Is.EqualTo(0));
		}

		[Test]
		public void Find_ShouldMatch_WhenMultipleInvocationHeadersMatchIncomingHeaders()
		{
			var invocationHeader = new Dictionary<string, IList<string>>
			{
				{ "header1", new List<string> { "a", "b", "c" } },
				{ "header2", new List<string> { "d", "e", "f" } }
			};
			var incomingHeader = new Dictionary<string, IList<string>>
			{
				{ "header1", new List<string> { "a" } },
				{ "header2", new List<string> { "d" } },
			};

			_invocationDb.Add(new Invocation { Method = "GET", Url = "/url1", Headers = invocationHeader });
			var invocations = _invocationDb.Find(new Invocation { Method = "GET", Url = "/url1", Headers = incomingHeader });
			Assert.That(invocations.Count, Is.EqualTo(1));
		}
	}
}
