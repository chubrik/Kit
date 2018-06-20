using Kit.Osm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kit.Tests
{
    [TestClass]
    public class OsmTests : TestsBase
    {
        private const string SrcPath = "../$data/antarctica.osm.pbf";

        [TestMethod]
        public void Validate()
        {
            TestInitialize($"{GetType().Name}.{nameof(Validate)}");
            OsmService.Validate(SrcPath);
        }

        [TestMethod]
        public void OsmPredicate()
        {
            TestInitialize($"{GetType().Name}.{nameof(OsmPredicate)}");
            var response = OsmService.Load(SrcPath, i => i.Tags.ContainsKey("place"));
            Assert.IsTrue(response.Nodes.Count > 100);
            Assert.IsTrue(response.Ways.Count > 5000);
            Assert.IsTrue(response.Relations.Count > 1000);
            Assert.IsTrue(response.MissedNodeIds.Count == 0);
            Assert.IsTrue(response.MissedWayIds.Count == 0);
            Assert.IsTrue(response.MissedRelationIds.Count == 0);
        }

        [TestMethod]
        public void GeoGroup()
        {
            TestInitialize($"{GetType().Name}.{nameof(GeoGroup)}");

            // https://www.openstreetmap.org/relation/2969204
            var title = "Vega Island";
            var osmId = 2969204;

            var group = GeoService.LoadGroup(SrcPath, cacheName: title, osmRelationId: osmId);
            Assert.IsTrue(group.Type == GeoType.Group);
            Assert.IsTrue(group.Id == osmId);
            Assert.IsTrue(group.Title == title);
            Assert.IsTrue(group.Tags["type"] == "multipolygon");
            Assert.IsTrue(group.Tags["place"] == "island");
            Assert.IsTrue(group.MemberIds.Count > 50);
            Assert.IsTrue(group.Members.Count == group.MemberIds.Count);
        }
    }
}
