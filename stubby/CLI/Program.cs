using System;
using CommandLine;

namespace stubby.CLI {

   internal class Program {
      private static readonly Arguments Arguments = new Arguments();
      private static readonly Stubby Stubby = new Stubby();

      private static void Main(string[] args) {
         if (!CommandLineParser.Default.ParseArguments(args, Arguments)) return;
         Stubby.Start(Arguments);
         while (true) {}
      }
   }

}