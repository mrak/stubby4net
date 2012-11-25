using System.Collections.Concurrent;

namespace stubby.Domain {

   public class EndpointDb {
      private readonly ConcurrentDictionary<uint, Endpoint> _dictionary = new ConcurrentDictionary<uint, Endpoint>();
      private readonly object _lock = new object();
      private uint _nextId;

      public void Create(Endpoint endpoint) {
         Create(new[]{endpoint});
      }

      public void Create(Endpoint[] endpoints) {}

      private uint NextId() {
         lock (_lock) {
            return _nextId++;
         }
      }
   }

}