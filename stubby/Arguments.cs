namespace stubby
{
   public class Arguments : IArguments {
      public uint Admin { get; set; }
      public uint Stubs { get; set; }
      public string Location { get; set; }
      public string Data { get; set; }
      public bool Watch { get; set; }
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
