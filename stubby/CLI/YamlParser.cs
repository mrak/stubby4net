using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;
using stubby.Domain;

namespace stubby.CLI {

   internal static class YamlParser {
      private const string CurrentDirectory = ".";
      private static string _fileDirectory = CurrentDirectory;

      public static Endpoint[] FromFile(string filename) {
         if (string.IsNullOrWhiteSpace(filename)) return new Endpoint[] {};

         _fileDirectory = Path.GetDirectoryName(filename);

         var yaml = new YamlStream();

         using (var streamReader = new StreamReader(filename)) {
            yaml.Load(streamReader);
         }

         return Parse(yaml);
      }

      public static Endpoint[] FromString(string data) {
         _fileDirectory = CurrentDirectory;

         var yaml = new YamlStream();

         using (var streamReader = new StringReader(data)) {
            yaml.Load(streamReader);
         }

         return Parse(yaml);
      }

      private static Endpoint[] Parse(YamlStream yaml) {
         var yamlEndpoints = (YamlSequenceNode) yaml.Documents[0].RootNode;

         return
            (from YamlMappingNode yamlEndpoint in yamlEndpoints.Children select ParseEndpoint(yamlEndpoint)).ToArray();
      }

      private static Endpoint ParseEndpoint(YamlMappingNode yamlEndpoint) {
         var endpoint = new Endpoint();

         foreach (var requestResponse in yamlEndpoint.Children) {
            switch (requestResponse.Key.ToString()) {
               case "request": {
                  endpoint.Request = ParseRequest((YamlMappingNode) requestResponse.Value);
                  break;
               }
               case "response": {
                  endpoint.Response = ParseResponse((YamlMappingNode) requestResponse.Value);
                  break;
               }
            }
         }

         return endpoint;
      }

      private static Request ParseRequest(YamlMappingNode yamlRequest) {
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
                  request.File = ParseFile(property.Value.ToString());
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

      private static string ParseFile(string file) {
         return Path.GetFullPath(Path.Combine(_fileDirectory, file));
      }

      private static List<string> ParseMethod(KeyValuePair<YamlNode, YamlNode> yamlMethod) {
         var methods = new List<string>();

         if (yamlMethod.Value.GetType() == typeof (YamlScalarNode))
            methods.Add(yamlMethod.Value.ToString().ToUpper());

         else if (yamlMethod.Value.GetType() == typeof (YamlSequenceNode))
            methods.AddRange(from method in (YamlSequenceNode) yamlMethod.Value select method.ToString().ToUpper());

         return methods;
      }

      private static Response ParseResponse(YamlMappingNode yamlResponse) {
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
                  response.File = ParseFile(property.Value.ToString());
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