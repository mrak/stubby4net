using System;
using System.Threading;
using CommandLine;

namespace stubby.CLI {

   internal class Program {
      private static void Main(string[] args) {
         var exitEvent = new ManualResetEvent(false);
         var arguments = new Arguments();

         if (!CommandLineParser.Default.ParseArguments(args, arguments)) return;
         if (arguments.Version) { Out.Log(Stubby.Version); return; }

         Console.CancelKeyPress += (sender, eventArgs) => {
            eventArgs.Cancel = true;
            exitEvent.Set();
         };

         var stubby = new Stubby(arguments);
         stubby.Start();

         Out.Info("Quit: Ctrl-c");
         Out.Linefeed();

         exitEvent.WaitOne();
         stubby.Stop();
      }
   }

}