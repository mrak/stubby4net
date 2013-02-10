using System.Collections.Concurrent;

namespace stubby.Domain {

   internal class EndpointDb {
      private readonly ConcurrentDictionary<uint, Endpoint> _dictionary = new ConcurrentDictionary<uint, Endpoint>();
      private readonly object _lock = new object();
      private uint _nextId;

      public void Purify(Endpoint endpoint) {
//         if (endpoint.Response == null) endpoint.Response = new Response();

      }

      public void Purify(Endpoint[] endpoints) {
         foreach (var endpoint in endpoints) Purify(endpoint);
      }

      public void Insert(Endpoint endpoint) {}

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