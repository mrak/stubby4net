using System;

namespace stubby.Exceptions {

   public class EndpointParsingException : Exception {
      public EndpointParsingException() {}

      public EndpointParsingException(string message) : base(message) {}

      public EndpointParsingException(string message, Exception inner) : base(message, inner) {}
   }

}