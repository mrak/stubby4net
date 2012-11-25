using System.Net;
using Moq;
using NUnit.Framework;
using stubby.Portals;

namespace stubby.unit {

   [TestFixture]
   public class PortalTest {
      [SetUp]
      public void BeforeEach() {
         _portal = new Portal();
      }

      private Portal _portal;

      [Test]
      public void BuildURI_ShouldConstructURIStringGivenLocationAndPort() {
         const string location = "abracadabra";
         const int port = 4242;

         var expected = "http://" + location + ":" + port + "/";

         var actual = _portal.BuildUri(location, port);

         Assert.AreEqual(expected, actual);
      }
   }

}