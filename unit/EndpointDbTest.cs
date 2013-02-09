using NUnit.Framework;
using stubby.Domain;

namespace stubby.unit {

   [TestFixture]
   public class EndpointDbTest {
      private EndpointDb _endpointDb;

      [SetUp]
      public void BeforeEach() {
         _endpointDb = new EndpointDb();
      }

      [Test]
      public void Purify_ShouldSetNullMethodToGET() {
         
      }
   }

}