using System.Collections.Generic;
using FubuObjectBlocks.Settings;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests.Settings
{
    [TestFixture]
    public class ObjectBlockCollectionTester
    {
        private IList<ObjectBlock> theBlocks;
        private ObjectBlockCollection theCollection;

        [SetUp]
        public void SetUp()
        {
            theBlocks = new List<ObjectBlock>();
            theCollection = new ObjectBlockCollection(theBlocks);
        }


        [Test]
        public void has_block()
        {
            theBlocks.Add(new ObjectBlock("Test"));
            theCollection.Has("Test").ShouldBeTrue();
        }

        [Test]
        public void does_not_have_block()
        {
            theCollection.Has("Test").ShouldBeFalse();
        }

        [Test]
        public void find_existing_block()
        {
            theBlocks.Add(new ObjectBlock("Test"));
            theCollection.Find("Test").ShouldNotBeNull();
        }

        [Test]
        public void find_non_existing_block()
        {
            theCollection.Find("Test").ShouldBeNull();
        }
    }
}