using NUnit.Framework;
using stubby.Contracts;
using stubby.Domain;

namespace stubby.unit {

   [TestFixture]
   public class EndointContractTest {
      [SetUp]
      public void BeforeEach() {
         _endpoint = new Endpoint {
            Request = {Url = "/something"},
            Response = {Status = 200}
         };
      }

      private Endpoint _endpoint;

      [Test]
      public void Verify_Method_ShouldReturnErrors_WhenMethodIsInvalid() {
         var expected = new[] {"request.method \"jeeber\" is not an accepted HTTP verb."};

         _endpoint.Request.Method = new[] {"jeeber"};

         var actual = EndpointContract.Verify(_endpoint);

         Assert.AreEqual(expected, actual);
      }

      [Test]
      public void Verify_Method_ShouldReturnErrors_WhenSomeMethodAreInvalid() {
         var expected = new[] {
            "request.method \"hello\" is not an accepted HTTP verb.",
            "request.method \"world\" is not an accepted HTTP verb."
         };
         _endpoint.Request.Method = new[] {"put", "hello", "world", "delete"};

         var actual = EndpointContract.Verify(_endpoint);

         Assert.AreEqual(expected, actual);
      }

      [Test]
      public void Verify_Method_ShouldReturnNoErrors_WhenAllMethodAreValid() {
         _endpoint.Request.Method = new[] {"put", "post", "delete"};

         var actual = EndpointContract.Verify(_endpoint);

         Assert.IsEmpty(actual);
      }

      [Test]
      public void Verify_Method_ShouldReturnNoErrors_WhenMethodIsValid() {
         var methods = new[] {"get", "put", "post", "delete", "patch", "options", "head"};

         foreach (var method in methods) {
            _endpoint.Request.Method = new[] {method};

            var actual = EndpointContract.Verify(_endpoint);

            Assert.IsEmpty(actual);
         }
      }

      [Test]
      public void Verify_Request_ShouldReturnError_WhenGivenANullRequest() {
         var expected = new[] {"request is required."};
         _endpoint.Request = null;

         var actual = EndpointContract.Verify(_endpoint);

         Assert.AreEqual(expected, actual);
      }

      [Test]
      public void Verify_Status_ShouldReturnErrors_WhenLessThan100() {
         var expected = new[] {"response.status must be between 100 and 599."};
         _endpoint.Response.Status = 42;

         var actual = EndpointContract.Verify(_endpoint);

         Assert.AreEqual(expected, actual);
      }

      [Test]
      public void Verify_Status_ShouldReturnErrors_WhenGreaterThan599() {
         var expected = new[] {"response.status must be between 100 and 599."};
         _endpoint.Response.Status = 600;

         var actual = EndpointContract.Verify(_endpoint);

         Assert.AreEqual(expected, actual);
      }

      [Test]
      public void Verify_Url_ShouldReturnError_WhenUrlDoesNotBeginWithSlash() {
         var expected = new[] {"request.url must begin with '/'."};
         _endpoint.Request.Url = "something";

         var actual = EndpointContract.Verify(_endpoint);

         Assert.AreEqual(expected, actual);
      }

      [Test]
      public void Verify_Url_ShouldReturnError_WhenUrlIsMissing() {
         var expected = new[] {"request.url is required."};

         var actual = EndpointContract.Verify(new Endpoint());

         Assert.AreEqual(expected, actual);
      }

      [Test]
      public void Verify_Url_ShouldReturnNoErrors_WhenUrlGiven() {
         var actual = EndpointContract.Verify(_endpoint);

         Assert.IsEmpty(actual);
      }
   }

}