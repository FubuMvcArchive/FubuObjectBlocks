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

            block.FindBlock<PropertyBlock>("name").Value.ShouldEqual("test");

            var feeds = block.FindBlock<CollectionItemBlock>("feed").Blocks.ToArray();
            
            feeds[0].ImplicitValue.ShouldEqual("http://localhost:8080");
            feeds[0].FindBlock<PropertyBlock>("mode").Value.ShouldEqual("fixed");

            feeds[1].ImplicitValue.ShouldEqual("http://localhost:8181");
            feeds[1].FindBlock<PropertyBlock>("mode").Value.ShouldEqual("float");
        }

        public class SuperComplexTarget
        {
            public string Name { get; set; }

            //TODO: what would we do if we had two objects both defining slightly different
            //block settings for an IEnumerable of the same type, ie in this case
            //another IEnumerable<Feed> with an ImplicitProperty = Mode 
            //would this be something we want to support?
            //if so, have to rethink how to look up CollectionConfigurations by type
            //as in the context of a single item such as Feed we cannot tell which
            //parent collection property it belongs to to lead back to a CollectionConfiguration
            //to inform us which Implict to use
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