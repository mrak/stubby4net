using NUnit.Framework;
using stubby.Domain;

namespace stubby.unit {

   [TestFixture]
   public class ContractTest {
      [SetUp]
      public void BeforeEach() {}

      [Test]
      public void Verify_Method_ShouldReturnErrors_WhenMethodIsInvalid() {
         var expected = new[] {"request.method must be a valid HTTP verb."};

         var endpoint = new Endpoint {Request = {Url = "/something", Method = "jeeber"}};

         var actual = Contract.Verify(endpoint);

         Assert.AreEqual(expected, actual);
      }

      [Test]
      public void Verify_Method_ShouldReturnNoErrors_WhenMethodIsValid() {
         var methods = new[] {"get", "put", "post", "delete", "patch", "options", "head"};

         foreach (var method in methods) {
            var endpoint = new Endpoint {Request = {Url = "/something", Method = method}};

            var actual = Contract.Verify(endpoint);

            Assert.IsEmpty(actual);
         }
      }

      [Test]
      public void Verify_Request_ShouldReturnError_WhenGivenANullRequest() {
         var expected = new[] {"request is required."};
         var endpoint = new Endpoint {Request = null};

         var actual = Contract.Verify(endpoint);

         Assert.AreEqual(expected, actual);
      }

      [Test]
      public void Verify_Url_ShouldReturnError_WhenUrlDoesNotBeginWithSlash() {
         var expected = new[] {"request.url must begin with '/'."};
         var endpoint = new Endpoint {Request = {Url = "something"}};

         var actual = Contract.Verify(endpoint);

         Assert.AreEqual(expected, actual);
      }

      [Test]
      public void Verify_Url_ShouldReturnError_WhenUrlIsMissing() {
         var expected = new[] {"request.url is required."};

         var actual = Contract.Verify(new Endpoint());

         Assert.AreEqual(expected, actual);
      }

      [Test]
      public void Verify_Url_ShouldReturnNoErrors_WhenUrlGiven() {
         var endpoint = new Endpoint {Request = {Url = "/something"}};

         var actual = Contract.Verify(endpoint);

         Assert.IsEmpty(actual);
      }
   }

}