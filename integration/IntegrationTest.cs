using NUnit.Framework;
using stubby;

namespace integration
{
   [TestFixture]
   public class IntegrationTest
   {
      [Test]
      public void TestMethod1() {
         var args = new Arguments {
            Admin = 8889,
            Stubs = 8882,
            Data = "../../YAML/e2e.yaml"
         };
         var stubby = new Stubby(args);
         stubby.Start();
         while (true) {}
         return;
      }
   }
}
