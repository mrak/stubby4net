using System.Collections.Concurrent;
using System.Linq;
using stubby.CLI;

namespace stubby.Domain {

   internal class EndpointDb {
      private const string Loaded = "Loaded{0} {1}";
      private readonly ConcurrentDictionary<uint, Endpoint> _dictionary = new ConcurrentDictionary<uint, Endpoint>();
      private readonly object _lock = new object();
      private uint _nextId;

      public void Insert(Endpoint endpoint) {
         if (!_dictionary.TryAdd(NextId(), endpoint)) return;

         var methods = endpoint.Request.Method.Aggregate("", (current, verb) => current + (" " + verb));
         Out.Notice(string.Format(Loaded, methods, endpoint.Request.Url));
      }

      public Endpoint Find(Endpoint incoming) {
         return (from stored in _dictionary where stored.Value.Equals(incoming) select stored.Value).FirstOrDefault();
      }

      public void Insert(Endpoint[] endpoints) {
         foreach (var endpoint in endpoints) Insert(endpoint);
      }

      private uint NextId() {
         lock (_lock) {
            return _nextId++;
         }
      }
   }

}