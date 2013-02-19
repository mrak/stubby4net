namespace stubby
{
   public class Arguments : IArguments {
      ///<summary>
      /// Port for admin portal. Defaults to 8889.
      ///</summary>
      public uint Admin { get; set; }
      ///<summary>
      /// Port for stubs portal. Defaults to 8882.
      ///</summary>
      public uint Stubs { get; set; }
      ///<summary>
      /// Hostname at which to bind stubby. Defaults to localhost.
      ///</summary>
      public string Location { get; set; }
      ///<summary>
      /// Data file to pre-load endpoints. YAML format.
      ///</summary>
      public string Data { get; set; }
      ///<summary>
      /// Monitor supplied data file for changes and reload endpoints if necessary. Defaults to false.
      ///</summary>
      public bool Watch { get; set; }
      ///<summary>
      /// Prevent stubby from logging to the console. Muted by default.
      ///</summary>
      public bool Mute { get; set; }

      public Arguments() {
         Admin = 8889;
         Stubs = 8882;
         Location = "localhost";
         Data = null;
         Mute = true;
         Watch = false;
      }
   }
}
