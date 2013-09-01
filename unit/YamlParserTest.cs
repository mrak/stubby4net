using System.IO;
using NUnit.Framework;
using stubby.CLI;

namespace unit {

    [TestFixture]
    public class YamlParserTest {
        [Test]
        public void ShouldParseMultipleEndpoints() {
            var endpoints = YamlParser.FromFile("../../YAML/multiple.yaml");

            Assert.AreEqual(3, endpoints.Count);
        }

        [Test]
        public void ShouldParseRequest_WithFile() {
            const string file = "../../YAML/request-file.yaml";

            var endpoint = YamlParser.FromFile(file)[0];
            Assert.AreEqual(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "path/to/file.txt")), endpoint.Request.File);
        }

        [Test]
        public void ShouldParseRequest_WithHeaders() {
            var endpoint = YamlParser.FromFile("../../YAML/request-headers.yaml")[0];

            Assert.AreEqual("text/xml", endpoint.Request.Headers["content-type"]);
            Assert.AreEqual("firefox(Mozilla)", endpoint.Request.Headers["user-agent"]);
        }

        [Test]
        public void ShouldParseRequest_WithMethod() {
            var endpoint = YamlParser.FromFile("../../YAML/request-method.yaml")[0];
            Assert.AreEqual(new[] { "DELETE" }, endpoint.Request.Method);
        }

        [Test]
        public void ShouldParseRequest_WithManyMethods() {
            var endpoint = YamlParser.FromFile("../../YAML/request-methods.yaml")[0];
            Assert.AreEqual(new[] { "GET", "HEAD" }, endpoint.Request.Method);
        }

        [Test]
        public void ShouldParseRequest_WithPostBody() {
            var endpoint = YamlParser.FromFile("../../YAML/request-post.yaml")[0];
            Assert.AreEqual("post body!", endpoint.Request.Post);
        }

        [Test]
        public void ShouldParseRequest_WithQuery() {
            var endpoint = YamlParser.FromFile("../../YAML/request-query.yaml")[0];

            Assert.AreEqual("tada", endpoint.Request.Query["first"]);
            Assert.AreEqual("voila", endpoint.Request.Query["second"]);
        }

        [Test]
        public void ShouldParseRequest_WithUrl() {
            var endpoint = YamlParser.FromFile("../../YAML/hello-world.yaml")[0];
            Assert.AreEqual("/hello/world", endpoint.Request.Url);
        }

        [Test]
        public void ShouldParseResponse_WithBody() {
            var endpoint = YamlParser.FromFile("../../YAML/response-body.yaml")[0];
            Assert.AreEqual("body contents!", endpoint.Response.Body);
        }

        [Test]
        public void ShouldParseResponse_WithFile() {
            const string file = "../../YAML/response-file.yaml";
            var endpoint = YamlParser.FromFile(file)[0];
            Assert.AreEqual(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "path/to/response/body")), endpoint.Response.File);
        }

        [Test]
        public void ShouldParseResponse_WithHeaders() {
            var endpoint = YamlParser.FromFile("../../YAML/response-headers.yaml")[0];

            Assert.AreEqual("application/json", endpoint.Response.Headers["content-type"]);
            Assert.AreEqual("application/json", endpoint.Response.Headers["accept"]);
        }

        [Test]
        public void ShouldParseResponse_WithLatency() {
            var endpoint = YamlParser.FromFile("../../YAML/response-latency.yaml")[0];
            Assert.AreEqual(987654321, endpoint.Response.Latency);
        }

        [Test]
        public void ShouldParseResponse_WithStatus() {
            var endpoint = YamlParser.FromFile("../../YAML/response-status.yaml")[0];
            Assert.AreEqual(204, endpoint.Response.Status);
        }

        [Test]
        public void ShouldReturnEmptyArray_GivenNullOrWhitespaceFileName() {
            Assert.IsEmpty(YamlParser.FromFile(""));
            Assert.IsEmpty(YamlParser.FromFile(null));
        }
    }
}