using System;
using System.IO;
using System.Threading;
using YamlDotNet.Core;
using stubby.CLI;
using stubby.Domain;
using stubby.Exceptions;
using stubby.Portals;

namespace stubby {

   public class Stubby {
      private Admin _admin;
      private Stubs _stubs;
      private Arguments _arguments;

      public void Start(Arguments arguments) {
         _arguments = arguments;
         Out.Mute = _arguments.Mute;

         SetUpPortals();
         StartPortals();
         Loop();
      }

      private void SetUpPortals() {
         var endpointsDb = new EndpointDb();

         LoadEndpoints(endpointsDb);

         _admin = new Admin(endpointsDb);
         _stubs = new Stubs(endpointsDb);
      }

      private void LoadEndpoints(EndpointDb endpointsDb) {
         var endpoints = new Endpoint[] {};
         
         try {
           endpoints = YamlParser.FromFile(_arguments.Data);
         } catch (FileNotFoundException) {
            Out.Warn("File '" + _arguments.Data + "' couldn't be found. Ignoring...");
         } catch (YamlException ex) {
            Out.Error(ex.Message);
            throw new EndpointParsingException("Could not parse endpoints due to YAML errors.", ex);
         }

         endpointsDb.Create(endpoints);
      }

      private void StartPortals() {
         ThreadPool.SetMaxThreads(50, 1000);
         ThreadPool.SetMinThreads(50, 50);

         _admin.Start(_arguments.Location, _arguments.Admin);
         _stubs.Start(_arguments.Location, _arguments.Stubs);
      }

      private void Loop() {
         while (true) {
            _admin.Listen();
            _stubs.Listen();
         }
      }
   }

}