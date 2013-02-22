using System;

namespace stubby.Portals {

   internal interface IPortal : IDisposable {
      void Stop();
      void Start(string location, uint port);
   }
}