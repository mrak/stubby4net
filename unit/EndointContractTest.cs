using System.Collections.Generic;
using NUnit.Framework;
using stubby.Domain;
using sut = stubby.Contracts.EndpointContract;

namespace unit {

    [TestFixture]
    public class EndointContractTest {
        [SetUp]
        public void BeforeEach() {
            _endpoint = new Endpoint
            {
                Request = { Url = "/something" },
                Response = { Status = 200 }
            };
        }

        private Endpoint _endpoint;

        [Test]
        public void Verify_Endpoint_ShouldReturnNew_IfNull() {
            var actual = sut.Verify((Endpoint) null);

            Assert.NotNull(actual);
        }

        [Test]
        public void Verify_Headers_ShouldReturnNew_IfNull() {
            _endpoint.Request.Headers = null;
            _endpoint.Response.Headers = null;

            var actual = sut.Verify(_endpoint);

            Assert.NotNull(actual.Request.Headers);
            Assert.NotNull(actual.Response.Headers);
        }

        [Test]
        public void Verify_Query_ShouldReturnNew_IfNull() {
            _endpoint.Request.Query = null;

            var actual = sut.Verify(_endpoint);

            Assert.NotNull(actual.Request.Query);
        }

        [Test]
        public void Verify_Method_ShouldCapitalizeAll() {
            _endpoint.Request.Method = new List<string> {"jeeber", "pUt", "Hello", "worLD", "deLete"};

            var actual = sut.Verify(_endpoint);

            Assert.AreEqual(new List<string> {"JEEBER", "PUT", "HELLO", "WORLD", "DELETE"}, actual.Request.Method);
        }

        [Test]
        public void Verify_Method_ShouldDefaultToGET() {
            _endpoint.Request.Method = null;

            var actual = sut.Verify(_endpoint);

            Assert.AreEqual(new List<string> { "GET" }, actual.Request.Method);

            _endpoint.Request.Method = new List<string>();

            var actual2 = sut.Verify(_endpoint);

            Assert.AreEqual(new List<string> { "GET" }, actual2.Request.Method);
        }

        [Test]
        public void Verify_Method_ShouldRemoveNullsAndWhitespace() {
            _endpoint.Request.Method = new List<string> {"jeeber", "", "  ", null, "   "};

            var actual = sut.Verify(_endpoint);

            Assert.AreEqual(new List<string> {"JEEBER"}, actual.Request.Method);
        }

        [Test]
        public void Verify_Request_ShouldReturnNew_IfNull() {
            _endpoint.Request = null;
            var actual = sut.Verify(_endpoint);

            Assert.NotNull(actual.Request);
        }

        [Test]
        public void Verify_Response_ShouldReturnNew_IfNull() {
            _endpoint.Response = null;
            var actual = sut.Verify(_endpoint);

            Assert.NotNull(actual.Response);
        }

        [Test]
        public void Verify_Status_ShouldBe200_When600() {
            const int expected = 200;
            _endpoint.Response.Status = 600;

            var actual = sut.Verify(_endpoint);

            Assert.AreEqual(expected, actual.Response.Status);
        }

        [Test]
        public void Verify_Status_ShouldBe200_WhenGreaterThan600() {
            const int expected = 200;
            _endpoint.Response.Status = 666;

            var actual = sut.Verify(_endpoint);

            Assert.AreEqual(expected, actual.Response.Status);
        }

        [Test]
        public void Verify_Status_ShouldBe200_WhenLessThan100() {
            const int expected = 200;
            _endpoint.Response.Status = 42;

            var actual = sut.Verify(_endpoint);

            Assert.AreEqual(expected, actual.Response.Status);
        }

        [Test]
        public void Verify_Status_ShouldBeGiven_WhenBetween100And600() {
            const int expected = 420;
            _endpoint.Response.Status = expected;

            var actual = sut.Verify(_endpoint);

            Assert.AreEqual(expected, actual.Response.Status);
        }

        [Test]
        public void Verify_Url_ShouldAddSlash_WhenUrlDoesNotBeginWithSlash() {
            const string expected = "/something";
            _endpoint.Request.Url = "something";

            var actual = sut.Verify(_endpoint);

            Assert.AreEqual(expected, actual.Request.Url);
        }

        [Test]
        public void Verify_Url_ShouldBeSlash_WhenUrlIsEmpty() {
            const string expected = "/";
            _endpoint.Request.Url = "";

            var actual = sut.Verify(_endpoint);

            Assert.AreEqual(expected, actual.Request.Url);
        }

        [Test]
        public void Verify_Url_ShouldBeSlash_WhenUrlIsNull() {
            const string expected = "/";
            _endpoint.Request.Url = null;

            var actual = sut.Verify(_endpoint);

            Assert.AreEqual(expected, actual.Request.Url);
        }
    }
}