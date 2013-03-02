namespace stubby
{
   public interface IArguments
   {
      uint Admin { get; set; }
      uint Stubs { get; set; }
      uint Tls { get; set; }
      string Location { get; set; }
      string Data { get; set; }
      bool Watch { get; set; }
      bool Mute { get; set; }
   }
}
