using NUnit.Framework;
using stubby.Contracts;
using stubby.Domain;

namespace stubby.unit {

   [TestFixture]
   public class EndointContractTest {
      [SetUp]
      public void BeforeEach() {}

      [Test]
      public void Verify_Method_ShouldReturnErrors_WhenMethodIsInvalid() {
         var expected = new[] { "request.method \"jeeber\" is not an accepted HTTP verb." };

         var endpoint = new Endpoint {Request = {Url = "/something", Method = new[] {"jeeber"}}};

         var actual = EndpointContract.Verify(endpoint);

         Assert.AreEqual(expected, actual);
      }

      [Test]
      public void Verify_Method_ShouldReturnNoErrors_WhenAllMethodAreValid() {
         var endpoint = new Endpoint {Request = {Url = "/something", Method = new[] {"put", "post", "delete"}}};

         var actual = EndpointContract.Verify(endpoint);

         Assert.IsEmpty(actual);
      }

      [Test]
      public void Verify_Method_ShouldReturnErrors_WhenSomeMethodAreInvalid() {
         var expected = new[] {
            "request.method \"hello\" is not an accepted HTTP verb.",
            "request.method \"world\" is not an accepted HTTP verb."
         };
         var endpoint = new Endpoint {Request = {Url = "/something", Method = new[] {"put", "hello", "world", "delete"}}};

         var actual = EndpointContract.Verify(endpoint);

         Assert.AreEqual(expected, actual);
      }

      [Test]
      public void Verify_Method_ShouldReturnNoErrors_WhenMethodIsValid() {
         var methods = new[] {"get", "put", "post", "delete", "patch", "options", "head"};

         foreach (var method in methods) {
            var endpoint = new Endpoint {Request = {Url = "/something", Method = new[] {method}}};

            var actual = EndpointContract.Verify(endpoint);

            Assert.IsEmpty(actual);
         }
      }

      [Test]
      public void Verify_Request_ShouldReturnError_WhenGivenANullRequest() {
         var expected = new[] {"request is required."};
         var endpoint = new Endpoint {Request = null};

         var actual = EndpointContract.Verify(endpoint);

         Assert.AreEqual(expected, actual);
      }

      [Test]
      public void Verify_Url_ShouldReturnError_WhenUrlDoesNotBeginWithSlash() {
         var expected = new[] {"request.url must begin with '/'."};
         var endpoint = new Endpoint {Request = {Url = "something"}};

         var actual = EndpointContract.Verify(endpoint);

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
         var endpoint = new Endpoint {Request = {Url = "/something"}};

         var actual = EndpointContract.Verify(endpoint);

         Assert.IsEmpty(actual);
      }
   }

}