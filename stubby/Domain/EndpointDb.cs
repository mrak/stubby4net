using System.Collections.Concurrent;
using System.Linq;

namespace stubby.Domain {

   internal class EndpointDb {
      private readonly ConcurrentDictionary<uint, Endpoint> _dictionary = new ConcurrentDictionary<uint, Endpoint>();
      private readonly object _lock = new object();
      private uint _nextId;

      public void Insert(Endpoint endpoint) {
         _dictionary.TryAdd(NextId(), endpoint);
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