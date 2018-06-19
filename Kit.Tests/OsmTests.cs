using Kit.Osm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kit.Tests
{
    [TestClass]
    public class OsmTests : TestsBase
    {
        private const string SrcPath = "../../$data/antarctica.osm.pbf";

        [TestMethod]
        public void Validate()
        {
            TestInitialize($"{GetType().Name}.{nameof(Validate)}");
            OsmService.Validate(SrcPath);
        }

        [TestMethod]
        public void Predicate()
        {
            TestInitialize($"{GetType().Name}.{nameof(Predicate)}");
            var response = OsmService.Load(SrcPath, i => i.Tags.ContainsKey("place"));
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Nodes.Count > 100);
            Assert.IsTrue(response.Ways.Count > 5000);
            Assert.IsTrue(response.Relations.Count > 1000);
            Assert.IsTrue(response.MissedNodeIds.Count == 0);
            Assert.IsTrue(response.MissedWayIds.Count == 0);
            Assert.IsTrue(response.MissedRelationIds.Count == 0);
        }
    }
}
