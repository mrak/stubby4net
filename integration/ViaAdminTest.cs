using System.IO;
using System.Net;
using System.Text;
using NUnit.Framework;
using stubby;

namespace integration
{
    [TestFixture]
    public class ViaAdminTest
    {
        private readonly Stubby _stubby = new Stubby(new Arguments
        {
            Admin = 9999,
            Stubs = 9992,
            Mute = true
        });

        [TestFixtureSetUp]
        public void Before()
        {
            _stubby.Start();
        }

        [TestFixtureTearDown]
        public void After()
        {
            _stubby.Stop();
        }

        [Test]
        public void AdminPostNewStub_ShouldReturnAccessibleLocationIdentifier()
        {
            var baseUri = "http://localhost:9999";
            var createStubRequest = WebRequest.Create(baseUri);
            createStubRequest.Method = "POST";
            createStubRequest.ContentType = "application/json";

            TestUtils.WritePost(createStubRequest,
                "[{\"request\": {\"url\":\"^/$\"},\"response\":{\"status\":200,\"body\": \"test\"}}]");

            var createStubResponse = (HttpWebResponse)createStubRequest.GetResponse();
            createStubResponse.Close();
            Assert.AreEqual(HttpStatusCode.Created, createStubResponse.StatusCode);

            var location = createStubResponse.Headers[HttpResponseHeader.Location];

            Assert.NotNull(location);

            var retrievalRequest = WebRequest.Create(baseUri + location);
            var retrievalResponse = (HttpWebResponse)retrievalRequest.GetResponse();
            retrievalResponse.Close();
            Assert.AreEqual(HttpStatusCode.OK, retrievalResponse.StatusCode);
        }

    }
}