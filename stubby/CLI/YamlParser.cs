using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;
using stubby.Domain;

namespace stubby.CLI {

   internal static class YamlParser {
      private const string CurrentDirectory = ".";

      public static Endpoint[] FromFile(string filename)
      {
         if (string.IsNullOrWhiteSpace(filename)) return new Endpoint[] {};

         var yaml = new YamlStream();

         using (var streamReader = new StreamReader(filename)) {
            yaml.Load(streamReader);
         }

         return Parse(yaml, filename);
      }

      public static Endpoint[] FromString(string data) {

         var yaml = new YamlStream();

         using (var streamReader = new StringReader(data)) {
            yaml.Load(streamReader);
         }

         return Parse(yaml, CurrentDirectory);
      }

      private static Endpoint[] Parse(YamlStream yaml, string filename) {
         var parsed = new List<Endpoint>();

         var yamlEndpoints = (YamlSequenceNode) yaml.Documents[0].RootNode;

         foreach (YamlMappingNode yamlEndpoint in yamlEndpoints.Children)
            parsed.Add(ParseEndpoint(yamlEndpoint, filename));

         return parsed.ToArray();
      }

      private static Endpoint ParseEndpoint(YamlMappingNode yamlEndpoint, string filename) {
         var endpoint = new Endpoint();

         foreach (var requestResponse in yamlEndpoint.Children) {
            switch (requestResponse.Key.ToString()) {
               case "request": {
                  endpoint.Request = ParseRequest((YamlMappingNode) requestResponse.Value, filename);
                  break;
               }
               case "response": {
                  endpoint.Response = ParseResponse((YamlMappingNode) requestResponse.Value, filename);
                  break;
               }
            }
         }

         return endpoint;
      }

      private static Request ParseRequest(YamlMappingNode yamlRequest, string filename) {
         var request = new Request();

         foreach (var property in yamlRequest) {
            switch (property.Key.ToString()) {
               case "url": {
                  request.Url = property.Value.ToString();
                  break;
               }
               case "method": {
                  request.Method = ParseMethod(property);
                  break;
               }
               case "file": {
                  request.File = ParseFile(property.Value.ToString(), filename);
                  break;
               }
               case "post": {
                  request.Post = property.Value.ToString();
                  break;
               }
               case "query": {
                  request.Query = ParseDictionary((YamlMappingNode) property.Value);
                  break;
               }
               case "headers": {
                  request.Headers = ParseDictionary((YamlMappingNode) property.Value, true);
                  break;
               }
            }
         }
         return request;
      }

      private static string ParseFile(string file, string source) {
         return Path.GetFullPath(Path.Combine(source, file));
      }

      private static List<string> ParseMethod(KeyValuePair<YamlNode, YamlNode> yamlMethod) {
         var methods = new List<string>();
         

         if (yamlMethod.Value.GetType() == typeof (YamlScalarNode))
            methods.Add(yamlMethod.Value.ToString().ToUpper());

         else if (yamlMethod.Value.GetType() == typeof (YamlSequenceNode))
            methods.AddRange(from method in (YamlSequenceNode) yamlMethod.Value select method.ToString().ToUpper());

         return methods;
      }

      private static Response ParseResponse(YamlMappingNode yamlResponse, string filename) {
         var response = new Response();

         foreach (var property in yamlResponse) {
            switch (property.Key.ToString()) {
               case "status": {
                  response.Status = ushort.Parse(property.Value.ToString());
                  break;
               }
               case "headers": {
                  response.Headers = ParseDictionary((YamlMappingNode) property.Value, true);
                  break;
               }
               case "latency": {
                  response.Latency = ulong.Parse(property.Value.ToString());
                  break;
               }
               case "body": {
                  response.Body = property.Value.ToString();
                  break;
               }
               case "file": {
                  response.File = ParseFile(property.Value.ToString(), filename);
                  break;
               }
            }
         }

         return response;
      }

      private static IDictionary<string, string> ParseDictionary(YamlMappingNode yamlMap, bool caseInsensitive = false) {
         IDictionary<string, string> dictionary = new Dictionary<string, string>();

         foreach (var property in yamlMap) {
            var key = property.Key.ToString();

            if (caseInsensitive) key = key.ToLower();

            dictionary.Add(key, property.Value.ToString());
         }

         return dictionary;
      }
   }

}