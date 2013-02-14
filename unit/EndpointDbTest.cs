using System.Collections.Generic;
using NUnit.Framework;
using stubby.Domain;

namespace unit {

   [TestFixture]
   public class EndpointDbTest {
      [SetUp]
      public void BeforeEach() {
         _endpointDb = new EndpointDb();
      }

      private EndpointDb _endpointDb;

      [Test]
      public void Find_ShouldRetreiveEndpoint_WhenMethodInList() {
         var inserted = new Endpoint {Request = new Request {Url = "/phantom", Method = new List<string> {"POST"}}};
         var incoming = new Endpoint {
            Request = new Request {Url = "/phantom", Method = new List<string> {"POST"}, Post = "A string!"}
         };

         _endpointDb.Insert(inserted);

         var actual = _endpointDb.Find(incoming);

         Assert.AreSame(inserted, actual);
      }

      [Test]
      public void Find_ShouldRetreiveEndpoint_WhenPostMatches() {
         var inserted = new Endpoint {Request = new Request {Url = "/phantom", Post = "A string!"}};
         var incoming = new Endpoint {
            Request = new Request {Url = "/phantom", Method = new List<string> {"GET"}, Post = "A string!"}
         };

         _endpointDb.Insert(inserted);

         var actual = _endpointDb.Find(incoming);

         Assert.AreSame(inserted, actual);
      }

      [Test]
      public void Find_ShouldRetrieveEndpoint_ByUrl() {
         var inserted = new Endpoint {Request = new Request {Url = "/phantom"}};
         var incoming = new Endpoint {Request = new Request {Url = "/phantom", Post = "A string!"}};

         _endpointDb.Insert(inserted);

         var actual = _endpointDb.Find(incoming);

         Assert.AreSame(inserted, actual);
      }

      [Test]
      public void Find_ShouldRetrieveEndpoint_WhenFileMatchesIncomingPost() {
         var inserted = new Endpoint {
            Request = new Request {Url = "/phantom", File = "../../Files/someFileContents.txt"}
         };
         var incoming = new Endpoint {
            Request = new Request {Url = "/phantom", Method = new List<string> {"GET"}, Post = "Some file contents!"}
         };

         _endpointDb.Insert(inserted);

         var actual = _endpointDb.Find(incoming);

         Assert.AreSame(inserted, actual);
      }

      [Test]
      public void Find_ShouldReturnNull_WhenFileDoesntMatchIncomingPost() {
         var inserted = new Endpoint {
            Request = new Request {Url = "/phantom", File = "../../Files/someFileContents.txt"}
         };
         var incoming = new Endpoint {
            Request = new Request {Url = "/phantom", Method = new List<string> {"GET"}, Post = "Some non-existant file contents!"}
         };

         _endpointDb.Insert(inserted);

         var actual = _endpointDb.Find(incoming);

         Assert.IsNull(actual);
      }

      [Test]
      public void Find_ShouldRetrieveEndpoint_WhenFileIsntFound_ButPostMatchesIncomingPost() {
         var inserted = new Endpoint {
            Request = new Request { Url = "/phantom", Post = "Some post contents!", File = "../../Files/nowhere.txt" }
         };
         var incoming = new Endpoint {
            Request = new Request {Url = "/phantom", Method = new List<string> {"GET"}, Post = "Some post contents!"}
         };

         _endpointDb.Insert(inserted);

         var actual = _endpointDb.Find(incoming);

         Assert.AreSame(inserted, actual);
      }

      [Test]
      public void Find_ShouldReturnNull_WhenFileIsntFound_AndPostDoesntMatchIncomingPost() {
         var inserted = new Endpoint {
            Request = new Request { Url = "/phantom", Post = "Nope!", File = "../../Files/nowhere.txt" }
         };
         var incoming = new Endpoint {
            Request = new Request {Url = "/phantom", Method = new List<string> {"GET"}, Post = "Some post contents!"}
         };

         _endpointDb.Insert(inserted);

         var actual = _endpointDb.Find(incoming);

         Assert.IsNull(actual);
      }

      [Test]
      public void Find_ShouldRetrieveEndpoint_WhenHeadersMatch() {
         var inserted = new Endpoint {
            Request =
               new Request {
                  Url = "/phantom",
                  Headers =
                     new Dictionary<string, string> {
                        {"Content-Type", "application/json"},
                        {"Content-Disposition", "attachment"},
                     }
               }
         };
         var incoming = new Endpoint {
            Request =
               new Request {
                  Url = "/phantom",
                  Method = new List<string> {"GET"},
                  Post = "A string!",
                  Headers =
                     new Dictionary<string, string> {
                        {"Content-Type", "application/json"},
                        {"Content-Disposition", "attachment"},
                        {"Server", "somethingSpecial"}
                     }
               }
         };

         _endpointDb.Insert(inserted);

         var actual = _endpointDb.Find(incoming);

         Assert.AreSame(inserted, actual);
      }

      [Test]
      public void Find_ShouldRetrieveEndpoint_WhenQueriesMatch() {
         var inserted = new Endpoint {
            Request =
               new Request {Url = "/phantom", Query = new Dictionary<string, string> {{"alpha", "a"}, {"beta", "b"},}}
         };
         var incoming = new Endpoint {
            Request =
               new Request {
                  Url = "/phantom",
                  Method = new List<string> {"GET"},
                  Post = "A string!",
                  Query = new Dictionary<string, string> {{"alpha", "a"}, {"beta", "b"}, {"kappa", "k"}}
               }
         };

         _endpointDb.Insert(inserted);

         var actual = _endpointDb.Find(incoming);

         Assert.AreSame(inserted, actual);
      }

      [Test]
      public void Find_ShouldReturnNull_WhenEndpointNotFound() {
         var inserted = new Endpoint {Request = new Request {Url = "/phantom"}};
         var incoming = new Endpoint {Request = new Request {Url = "/of/the/opera", Post = "A string!"}};

         _endpointDb.Insert(inserted);

         var actual = _endpointDb.Find(incoming);

         Assert.IsNull(actual);
      }

      [Test]
      public void Find_ShouldReturnNull_WhenHeadersDontMatch() {
         var inserted = new Endpoint {
            Request =
               new Request {
                  Url = "/phantom",
                  Headers =
                     new Dictionary<string, string> {
                        {"Content-Type", "application/json"},
                        {"Content-Disposition", "attachment"},
                     }
               }
         };
         var incoming = new Endpoint {
            Request =
               new Request {
                  Url = "/phantom",
                  Method = new List<string> {"GET"},
                  Post = "A string!",
                  Headers =
                     new Dictionary<string, string> {
                        {"Content-Type", "application/xml"},
                        {"Content-Disposition", "attachment"},
                        {"Server", "somethingSpecial"}
                     }
               }
         };

         _endpointDb.Insert(inserted);

         var actual = _endpointDb.Find(incoming);

         Assert.IsNull(actual);
      }

      [Test]
      public void Find_ShouldReturnNull_WhenMethodNotInList() {
         var inserted = new Endpoint {
            Request = new Request {Url = "/phantom", Method = new List<string> {"POST", "HEAD"}}
         };
         var incoming = new Endpoint {
            Request = new Request {Url = "/phantom", Method = new List<string> {"GET"}, Post = "A string!"}
         };

         _endpointDb.Insert(inserted);

         var actual = _endpointDb.Find(incoming);

         Assert.IsNull(actual);
      }

      [Test]
      public void Find_ShouldReturnNull_WhenPostIsDifferent() {
         var inserted = new Endpoint {Request = new Request {Url = "/phantom", Post = "Tsk tsk."}};
         var incoming = new Endpoint {
            Request = new Request {Url = "/phantom", Method = new List<string> {"GET"}, Post = "A string!"}
         };

         _endpointDb.Insert(inserted);

         var actual = _endpointDb.Find(incoming);

         Assert.IsNull(actual);
      }

      [Test]
      public void Find_ShouldReturnNull_WhenQueryDoesntMatch() {
         var inserted = new Endpoint {
            Request =
               new Request {Url = "/phantom", Query = new Dictionary<string, string> {{"alpha", "a"}, {"beta", "b"},}}
         };
         var incoming = new Endpoint {
            Request =
               new Request {
                  Url = "/phantom",
                  Method = new List<string> {"GET"},
                  Post = "A string!",
                  Query = new Dictionary<string, string> {{"alpha", "c"}, {"beta", "b"}, {"kappa", "k"}}
               }
         };

         _endpointDb.Insert(inserted);

         var actual = _endpointDb.Find(incoming);

         Assert.IsNull(actual);
      }
   }

}