﻿using Kit.Osm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kit.Tests
{
    [TestClass]
    public class OsmTests : TestsBase
    {
        [TestMethod]
        public void Validate()
        {
            TestInitialize($"{GetType().Name}.{nameof(Validate)}");
            OsmService.Validate("../../$data/antarctica.osm.pbf");
        }
    }
}
