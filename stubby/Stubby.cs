using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using stubby.CLI;
using stubby.Domain;
using stubby.Exceptions;
using stubby.Portals;

namespace stubby {

   public class Stubby {
      private readonly Admin _admin;
      private readonly EndpointDb _endpointDb;
      private readonly Stubs _stubs;
      private IArguments _arguments;

      public Stubby() {
         _endpointDb = new EndpointDb();
         _admin = new Admin(_endpointDb);
         _stubs = new Stubs(_endpointDb);
      }

      public void Start() {
         StartPortals();
      }

      public void Start(IArguments arguments) {
         _arguments = arguments;
         Out.Mute = _arguments.Mute;

         LoadEndpoints();
         StartPortals();
      }

      public void Stop() {
         _admin.Stop();
         _stubs.Stop();
      }

      public IList<Endpoint> GetAll() {
         return _endpointDb.Fetch();
      }

      public Endpoint Get(uint id) {
         return _endpointDb.Fetch(id);
      }

      public bool Replace(uint id, Endpoint endpoint) {
         return _endpointDb.Replace(id, endpoint);
      }

      public bool Replace(IEnumerable<KeyValuePair<uint, Endpoint>> endpoints) {
         return _endpointDb.Replace(endpoints);
      }

      public bool Delete(uint id) {
         return _endpointDb.Delete(id);
      }

      public void DeleteAll() {
         _endpointDb.Delete();
      }

      public bool Add(Endpoint endpoint, out uint id) {
         return _endpointDb.Insert(endpoint, out id);
      }

      public bool Add(IEnumerable<Endpoint> endpoints, out IList<uint> ids) {
         return _endpointDb.Insert(endpoints, out ids);
      }

      private void LoadEndpoints() {
         IList<Endpoint> endpoints = new List<Endpoint>();

         try {
            endpoints = YamlParser.FromFile(_arguments.Data);
         } catch (FileNotFoundException) {
            Out.Warn("File '" + _arguments.Data + "' couldn't be found. Ignoring...");
         } catch (YamlException ex) {
            Out.Error(ex.Message);
            throw new EndpointParsingException("Could not parse endpoints due to YAML errors.", ex);
         }

         _endpointDb.Insert(endpoints);
      }

      private void StartPortals() {
         Out.Linefeed();
         _admin.Start(_arguments.Location, _arguments.Admin);
         _stubs.Start(_arguments.Location, _arguments.Stubs);
         Out.Linefeed();
      }
   }

}