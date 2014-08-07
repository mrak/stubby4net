using System;
using System.Collections.Generic;
using System.Linq;

namespace stubby.Domain
{
	internal class InvocationDb
	{
		private readonly IDictionary<string, IDictionary<string, IList<Invocation>>> _invocations = new Dictionary<string, IDictionary<string, IList<Invocation>>>();

		public void Add(Invocation invocation)
		{
			IDictionary<string, IList<Invocation>> verbInvocations;
			if (!_invocations.TryGetValue(invocation.Method, out verbInvocations))
			{
				lock (_invocations)
				{
					if (!_invocations.TryGetValue(invocation.Method, out verbInvocations))
					{
						verbInvocations = new Dictionary<string, IList<Invocation>>();
						verbInvocations[invocation.Url] = new List<Invocation>();
						_invocations[invocation.Method] = verbInvocations;
					}
				}
			}

			IList<Invocation> urlInvocations;
			if (!verbInvocations.TryGetValue(invocation.Url, out urlInvocations))
			{
				lock (verbInvocations)
				{
					if (!verbInvocations.TryGetValue(invocation.Url, out urlInvocations))
					{
						urlInvocations = new List<Invocation>();
						verbInvocations[invocation.Url] = urlInvocations;
					}
				}
			}

			lock (urlInvocations)
				urlInvocations.Add(invocation);
		}

		private IList<Invocation> GetInvocations(Invocation incoming)
		{
			IDictionary<string, IList<Invocation>> verbInvocations;
			if (!_invocations.TryGetValue(incoming.Method, out verbInvocations))
				return new List<Invocation>();

			IList<Invocation> invocations;
			if (!verbInvocations.TryGetValue(incoming.Url, out invocations))
				return new List<Invocation>();

			return invocations;
		}

		public IList<Invocation> Find(Invocation incoming)
		{
			var invocations = GetInvocations(incoming);
			lock (invocations)
				return invocations.Where(i => IsMatched(i, incoming)).ToList();
		}

		private static bool IsMatched(Invocation invocation, Invocation incoming)
		{
			if (!IsMatched(invocation.Headers, incoming.Headers))
				return false;

			if (!IsMatched(invocation.Query, incoming.Query))
				return false;

			return String.IsNullOrEmpty(incoming.Post) ||
			       String.Equals(invocation.Post, incoming.Post, StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsMatched(IDictionary<string, IList<string>> invocation, IDictionary<string, IList<string>> incoming)
		{
			if (invocation == incoming)
				return true;

			if (invocation == null || incoming == null)
				return false;

			foreach (var incomingEntry in incoming)
			{
				var key = incomingEntry.Key;
				if (!invocation.ContainsKey(key))
					return false;

				if (!incomingEntry.Value.Any())
					continue;

				if (!incomingEntry.Value.All(x => invocation[key].Any(y => String.Equals(x, y, StringComparison.OrdinalIgnoreCase))))
					return false;
			}

			return true;
		}
	}
}
