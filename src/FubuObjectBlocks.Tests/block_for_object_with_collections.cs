using System.Collections.Generic;
using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests
{
    [TestFixture]
    public class block_for_object_with_collections
    {
        [Test]
        public void sets_the_properties()
        {
            var target = new SuperComplexTarget
                {
                    Name = "test",
                    Feeds = new[]
                        {
                            new Feed { Url = "http://localhost:8080", Mode = "fixed"},
                            new Feed { Url = "http://localhost:8181", Mode = "float"}
                        }
                };

            var serializer = ObjectBlockSerializer.Basic();
            var block = serializer.BlockFor(target, new ObjectBlockSettings());

            block.FindProperty("name").Block.Value.ShouldEqual("test");

            var feeds = block.FindProperty("feed").Blocks.ToArray();
            
            feeds[0].Value.ShouldEqual("http://localhost:8080");
            feeds[0].FindProperty("mode").Block.Value.ShouldEqual("fixed");

            feeds[1].Value.ShouldEqual("http://localhost:8181");
            feeds[1].FindProperty("mode").Block.Value.ShouldEqual("float");
        }

        public class SuperComplexTarget
        {
            public string Name { get; set; }

            [BlockSettings(ExpressAs = "feed", ImplicitProperty = "Url")]
            public IEnumerable<Feed> Feeds { get; set; }
        }

        public class Feed
        {
            public string Url { get; set; }
            public string Mode { get; set; }
        }
    }
}