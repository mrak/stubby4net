using CommandLine;

namespace stubby.CLI {

   internal class Program {
      private static void Main(string[] args) {
         var arguments = new Arguments();
         if (!CommandLineParser.Default.ParseArguments(args, arguments)) return;

         var stubby = new Stubby(arguments);
         stubby.Start();
         while (true) {}
      }
   }

}