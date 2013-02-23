using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using stubby.CLI;
using Defaults = stubby.Contracts.EndpointContract;

namespace stubby.Domain {

   internal class EndpointDb {
      private const string Loaded = "Loaded{0} {1}";
      private readonly ConcurrentDictionary<uint, Endpoint> _dictionary = new ConcurrentDictionary<uint, Endpoint>();
      private readonly object _lock = new object();
      private uint _nextId;

      public bool Insert(Endpoint endpoint) {
         uint i;
         return Insert(endpoint, out i);
      }

      public bool Insert(Endpoint endpoint, out uint id) {
         id = NextId();
         var verified = Defaults.Verify(endpoint);

         if (!_dictionary.TryAdd(id, verified)) return false;

         var methods = verified.Request.Method.Aggregate("", (current, verb) => current + (" " + verb));
         Out.Notice(string.Format(Loaded, methods, verified.Request.Url));
         return true;
      }

      public bool Insert(IEnumerable<Endpoint> endpoints) {
         IList<uint> ids;
         return Insert(endpoints, out ids);
      }

      public bool Insert(IEnumerable<Endpoint> endpoints, out IList<uint> ids) {
         ids = new List<uint>();
         foreach (var endpoint in endpoints) {
            uint id;
            if (!Insert(endpoint, out id)) return false;
            ids.Add(id);
         }
         return true;
      }

      public Endpoint Find(Endpoint incoming) {
         return (from stored in _dictionary where stored.Value.Matches(incoming) select stored.Value).FirstOrDefault();
      }

      public Endpoint Fetch(uint id) {
         Endpoint fetched;
         _dictionary.TryGetValue(id, out fetched);
         return fetched;
      }

      public bool Replace(uint id, Endpoint endpoint) {
         if (!_dictionary.ContainsKey(id)) return false;

         _dictionary[id] = Defaults.Verify(endpoint);
         return true;
      }

      public bool Replace(IEnumerable<KeyValuePair<uint, Endpoint>> endpoints) {
         return endpoints.All(pair => Replace(pair.Key, pair.Value));
      }

      public IList<Endpoint> Fetch() {
         return _dictionary.Select(endpoint => endpoint.Value).ToList();
      }

      public void Delete() {
         _dictionary.Clear();
      }

      public bool Delete(uint id) {
         Endpoint removed;
         return _dictionary.TryRemove(id, out removed);
      }

      private uint NextId() {
         lock (_lock) {
            return _nextId++;
         }
      }
   }

}