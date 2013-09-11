using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests
{
    [TestFixture]
    public class ObjectBlockValuesTester
    {
        private ObjectBlock theBlock;
        private ObjectBlockValues theValues;

        [SetUp]
        public void SetUp()
        {
            theBlock = new ObjectBlock();
            theValues = new ObjectBlockValues(theBlock, typeof(Solution));
        }

        [Test]
        public void has_key()
        {
            theBlock.AddBlock(new PropertyBlock("test"));
            theValues.Has("test").ShouldBeTrue();
        }

        [Test]
        public void has_key_negative()
        {
            theValues.Has("test").ShouldBeFalse();
        }

        [Test]
        public void gets_property_values()
        {
            theBlock.AddBlock(new PropertyBlock("test") {Value = "1234"});
            theValues.Get("test").ShouldEqual("1234");
        }

        [Test]
        public void has_child()
        {
            var child = new ObjectBlock("test");
            child.AddBlock(new PropertyBlock("sub-property"));
            theBlock.AddBlock(child);

            theValues.HasChild("test").ShouldBeTrue();
        }

        [Test]
        public void has_child_negative_no_properties()
        {
            var property = new PropertyBlock("test");
            theBlock.AddBlock(property);

            theValues.HasChild("test").ShouldBeFalse();
        }

        [Test]
        public void has_child_negative()
        {
            theValues.HasChild("test").ShouldBeFalse();
        }

        [Test]
        public void get_child_for_existing_values()
        {
            var child = new ObjectBlock("test");
            child.AddBlock(new PropertyBlock("sub-property"));
            theBlock.AddBlock(child);

            theValues.GetChild("test").Has("sub-property").ShouldBeTrue();
        }

        [Test]
        public void get_children_for_non_existing_values()
        {
            theValues.GetChild("test").ShouldNotBeNull();
        }

        [Test]
        public void get_children_for_existing_values()
        {
            var block1 = new ObjectBlock();
            block1.AddBlock(new PropertyBlock("Url") { Value = "url1" });

            var block2 = new ObjectBlock();
            block2.AddBlock(new PropertyBlock("Url") { Value = "url2" });

            var collection = new CollectionBlock("feeds");
            collection.AddBlock(block1);
            collection.AddBlock(block2);
            theBlock.AddBlock(collection);

            var values = theValues.GetChildren("Feeds").ToArray();

            values[0].Get("Url").ShouldEqual("url1");
            values[1].Get("Url").ShouldEqual("url2");
        }

        [Test]
        public void get_mapped_children_for_existing_values()
        {
            var block1 = new ObjectBlock();
            block1.AddBlock(new PropertyBlock("Url") { Value = "url1" });

            var block2 = new ObjectBlock();
            block2.AddBlock(new PropertyBlock("Url") { Value = "url2" });

            var collection = new CollectionBlock("feed");
            collection.AddBlock(block1);
            collection.AddBlock(block2);
            theBlock.AddBlock(collection);

            var mappedValueSource = new ObjectBlockValues(theBlock, new FeedObjectSettings(), typeof(Solution));

            var values = mappedValueSource.GetChildren("Feeds").ToArray();

            values[0].Get("Url").ShouldEqual("url1");
            values[1].Get("Url").ShouldEqual("url2");
        }

        [Test]
        public void get_implicit_value()
        {
            var feedBlock = new ObjectBlock("feed")
            {
                ImplicitValue = "http://www.google.com"
            };

            var mappedValueSource = new ObjectBlockValues(feedBlock, new FeedObjectSettings(), typeof(FeedObject));

            var value = "";
            mappedValueSource.Value("Url", x =>
            {
                value = x.RawValue as string;
            });

            value.ShouldEqual("http://www.google.com");
        }

        public class FeedObject
        {
            public string Url { get; set; }
        }

        public class Solution
        {
            public IEnumerable<FeedObject> Feeds { get; set; }
        }

        public class FeedObjectSettings : IObjectBlockSettings
        {
            public string Collection(Type type, string key)
            {
                return "feed";
            }

            public Accessor ImplicitValue(Type type, string key)
            {
                return ReflectionHelper.GetAccessor<FeedObject>(x => x.Url);
            }

            public Accessor FindImplicitValue(Type type)
            {
                throw new NotImplementedException();
            }

            public Type FindCollectionType(Type type, string key)
            {
                return type;
            }

            public IEnumerable<string> KnownCollectionNames()
            {
                yield return "feed";
            }
        }
    }
}