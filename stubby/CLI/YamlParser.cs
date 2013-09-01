using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.RepresentationModel;
using stubby.Domain;

namespace stubby.CLI {

    internal static class YamlParser {
        private const string CurrentDirectory = ".";
        private static string _fileDirectory = CurrentDirectory;

        public static IList<Endpoint> FromFile(string filename) {
            if(string.IsNullOrWhiteSpace(filename))
                return new List<Endpoint>();

            _fileDirectory = Path.GetDirectoryName(filename);

            var yaml = new YamlStream();

            using(var streamReader = new StreamReader(filename)) {
                yaml.Load(streamReader);
            }

            return Parse(yaml);
        }

        public static IList<Endpoint> FromString(string data) {
            _fileDirectory = CurrentDirectory;

            var yaml = new YamlStream();

            using(var streamReader = new StringReader(data)) {
                yaml.Load(streamReader);
            }

            return Parse(yaml);
        }

        private static IList<Endpoint> Parse(YamlStream yaml) {
            var yamlEndpoints = (YamlSequenceNode) yaml.Documents[0].RootNode;

            return (from YamlMappingNode yamlEndpoint in yamlEndpoints select ParseEndpoint(yamlEndpoint)).ToList();
        }

        private static Endpoint ParseEndpoint(YamlMappingNode yamlEndpoint) {
            var endpoint = new Endpoint();

            foreach(var requestResponse in yamlEndpoint.Children) {
                switch(requestResponse.Key.ToString()) {
                    case "request":
                        {
                            endpoint.Request = ParseRequest((YamlMappingNode) requestResponse.Value);
                            break;
                        }
                    case "response":
                        {
                            endpoint.Responses = ParseResponses(requestResponse);
                            break;
                        }
                }
            }

            return endpoint;
        }

        private static Request ParseRequest(YamlMappingNode yamlRequest) {
            var request = new Request();

            foreach(var property in yamlRequest) {
                switch(property.Key.ToString()) {
                    case "url":
                        {
                            request.Url = ParseString(property);
                            break;
                        }
                    case "method":
                        {
                            request.Method = ParseMethod(property);
                            break;
                        }
                    case "file":
                        {
                            request.File = ParseFile(property);
                            break;
                        }
                    case "post":
                        {
                            request.Post = ParseString(property);
                            break;
                        }
                    case "query":
                        {
                            request.Query = ParseCollection(property);
                            break;
                        }
                    case "headers":
                        {
                            request.Headers = ParseCollection(property, false);
                            break;
                        }
                }
            }
            return request;
        }

        private static string ParseString(KeyValuePair<YamlNode, YamlNode> property) {
            return property.Value.ToString().TrimEnd(new[]
            {
                ' ',
                '\t',
                '\n',
                '\r'
            });
        }

        private static string ParseFile(KeyValuePair<YamlNode, YamlNode> property) {
            return Path.GetFullPath(Path.Combine(_fileDirectory, property.Value.ToString()));
        }

        private static List<string> ParseMethod(KeyValuePair<YamlNode, YamlNode> yamlMethod) {
            var methods = new List<string>();

            if(yamlMethod.Value.GetType() == typeof(YamlScalarNode))
                methods.Add(yamlMethod.Value.ToString().ToUpper());
            else if(yamlMethod.Value.GetType() == typeof(YamlSequenceNode))
                methods.AddRange(from method in (YamlSequenceNode) yamlMethod.Value select method.ToString().ToUpper());

            return methods;
        }

        private static IList<Response> ParseResponses(KeyValuePair<YamlNode, YamlNode> yamlResponse){
            if(yamlResponse.Value.GetType() == typeof(YamlSequenceNode))
                return (from response in (YamlSequenceNode) yamlResponse.Value select ParseResponse((YamlMappingNode) response)).ToList();            else
            return new List<Response> {ParseResponse((YamlMappingNode) yamlResponse.Value)};
        }

        private static Response ParseResponse(YamlMappingNode yamlResponse) {
            var response = new Response();

            foreach(var property in yamlResponse) {
                switch(property.Key.ToString()) {
                    case "status":
                        {
                            response.Status = ushort.Parse(property.Value.ToString());
                            break;
                        }
                    case "headers":
                        {
                            response.Headers = ParseCollection(property, false);
                            break;
                        }
                    case "latency":
                        {
                            response.Latency = ulong.Parse(property.Value.ToString());
                            break;
                        }
                    case "body":
                        {
                            response.Body = ParseString(property);
                            break;
                        }
                    case "file":
                        {
                            response.File = ParseFile(property);
                            break;
                        }
                }
            }

            return response;
        }

        private static NameValueCollection ParseCollection(KeyValuePair<YamlNode, YamlNode> property, bool caseSensitive = true) {
            var keyValuePairs = (YamlMappingNode) property.Value;
            var collection = new NameValueCollection();

            foreach(var keyValuePair in keyValuePairs) {
                var key = keyValuePair.Key.ToString();
                var value = keyValuePair.Value.ToString();

                if(property.Key.ToString().Equals("headers") &&
                    key.Equals("authorization", StringComparison.InvariantCultureIgnoreCase) && value.Contains(":")) {
                    value = value.Replace("Basic ", "");
                    value = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
                }

                collection.Add(caseSensitive ? key : key.ToLower(), value);
            }

            return collection;
        }
    }
}