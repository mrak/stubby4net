using System;
using CommandLine;
using CommandLine.Text;

namespace stubby.CLI {

   internal class Arguments : CommandLineOptionsBase, IArguments  {
      [Option("a", "admin", DefaultValue = (uint) 8889, HelpText = "Port for admin portal. Defaults to 8889.")]
      public uint Admin { get; set; }

      [Option("s", "stubs", DefaultValue = (uint) 8882, HelpText = "Port for stubs portal. Defaults to 8882.")]
      public uint Stubs { get; set; }

      [Option("t", "tls", DefaultValue = (uint) 7443, HelpText = "Port for https stubs portal. Defaults to 7443.")]
      public uint Tls { get; set; }

      [Option("l", "location", DefaultValue = "localhost", HelpText = "Hostname at which to bind stubby.")]
      public string Location { get; set; }

      [Option("d", "data", HelpText = "Data file to pre-load endpoints. YAML format.")]
      public string Data { get; set; }

      [Option("w", "watch", HelpText = "Monitor supplied data file for changes and reload endpoints if necessary.")]
      public bool Watch { get; set; }

      [Option("m", "mute", HelpText = "Prevent stubby from logging to the console.")]
      public bool Mute { get; set; }

      [Option("v", "version", HelpText = "Print stubby's version number.")]
      public bool Version { get; set; }

      [HelpOption]
      public string GetUsage() {
         var help = new HelpText {
            Heading = new HeadingInfo("stubby", Stubby.Version),
            Copyright = new CopyrightInfo("Eric Mrak", DateTime.Now.Year),
            AddDashesToOption = true
         };
         help.AddPreOptionsLine("Apache 2.0 License");
         help.AddOptions(this);
         return help;
      }
   }

}