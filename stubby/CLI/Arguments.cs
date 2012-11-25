using CommandLine;
using CommandLine.Text;

namespace stubby.CLI {

   public class Arguments : CommandLineOptionsBase {
      [Option("a", "admin", DefaultValue = (uint) 8889, HelpText = "Port for admin portal. Defaults to 8889.")]
      public uint Admin { get; set; }

      [Option("s", "stubs", DefaultValue = (uint) 8882, HelpText = "Port for stubs portal. Defaults to 8882.")]
      public uint Stubs { get; set; }

      [Option("l", "location", DefaultValue = "localhost", HelpText = "Hostname at which to bind stubby.")]
      public string Location { get; set; }

      [Option("d", "data", HelpText = "Data file to pre-load endpoints. YAML format.")]
      public string Data { get; set; }

      [Option("w", "watch", HelpText = "Monitor supplied data file for changes and reload endpoints if necessary.")]
      public bool Watch { get; set; }

      [Option("m", "mute", HelpText = "Monitor supplied data file for changes and reload endpoints if necessary.")]
      public bool Mute { get; set; }

      [HelpOption]
      public string GetUsage() {
         var help = new HelpText {
            Heading = new HeadingInfo("stubby", "1.0.0"),
            Copyright = new CopyrightInfo("Eric Mrak", 2012),
            AddDashesToOption = true
         };
         help.AddPreOptionsLine("Apache 2.0 License");
         help.AddOptions(this);
         return help;
      }
   }

}