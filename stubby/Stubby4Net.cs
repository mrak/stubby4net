using System;
using System.IO;
using System.Threading;
using CommandLine;
using YamlDotNet.Core;
using stubby.CLI;
using stubby.Domain;
using stubby.Portals;

namespace stubby {

   public class Stubby4Net {
      private static Admin _admin;
      private static Stubs _stubs;
      private static readonly Arguments Arguments = new Arguments();

      private static void Main(string[] args) {
         if (!CommandLineParser.Default.ParseArguments(args, Arguments)) return;

         SetUpPortals();
         StartPortals();
         Loop();
      }

      private static void SetUpPortals() {
         var endpoints = new EndpointDb();

         LoadEndpoints(endpoints);

         _admin = new Admin(endpoints);
         _stubs = new Stubs(endpoints);
      }

      private static void LoadEndpoints(EndpointDb endpoints) {
         try {
            endpoints.Create(YamlParser.FromFile(Arguments.Data));
         } catch (FileNotFoundException) {
            Out.Warn("File '" + Arguments.Data + "' couldn't be found. Ignoring...");
         } catch (YamlException ex) {
            Out.Error(ex.Message);
            Environment.Exit(1);
         }
      }

      private static void StartPortals() {
         ThreadPool.SetMaxThreads(50, 1000);
         ThreadPool.SetMinThreads(50, 50);

         _admin.Start(Arguments.Location, Arguments.Admin);
         _stubs.Start(Arguments.Location, Arguments.Stubs);
      }

      private static void Loop() {
         while (true) {
            _admin.Listen();
            _stubs.Listen();
         }
      }
   }

}