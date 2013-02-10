using NUnit.Framework;
using stubby.Portals;

namespace unit {

   [TestFixture]
   public class PortalUtilsTest {

      [Test]
      public void BuildURI_ShouldConstructURIStringGivenLocationAndPort() {
         const string location = "abracadabra";
         const int port = 4242;

         var expected = "http://" + location + ":" + port + "/";

         var actual = PortalUtils.BuildUri(location, port);

         Assert.AreEqual(expected, actual);
      }
   }

}