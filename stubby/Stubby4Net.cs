using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using CommandLine;
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

         endpoints.Create(YamlParser.Parse(Arguments.Data));
         
         _admin = new Admin(endpoints);
         _stubs = new Stubs(endpoints);
      }

      private static void StartPortals() {
         ThreadPool.SetMaxThreads(50, 1000);
         ThreadPool.SetMinThreads(50, 50);

         _admin.Start(Arguments.Location, Arguments.Admin);
         _stubs.Start(Arguments.Location, Arguments.Stubs);
      }

      private static void Loop() {
         while (true)
         {
            _admin.Listen();
            _stubs.Listen();
         }
      }
   }

}