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
        private ObjectBlockValues<Solution> theValues;

        [SetUp]
        public void SetUp()
        {
            theBlock = new ObjectBlock();
            theValues = new ObjectBlockValues<Solution>(theBlock);
        }

        [Test]
        public void has_key()
        {
            theBlock.AddProperty(new PropertyBlock("test"));
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
            theBlock.AddProperty(new PropertyBlock("test") { Block = new ObjectBlock { Value = "1234" }});
            theValues.Get("test").ShouldEqual("1234");
        }

        [Test]
        public void has_child()
        {
            var property = new PropertyBlock("test") {Block = new ObjectBlock()};
            property.Block.AddProperty(new PropertyBlock("sub-property"));
            theBlock.AddProperty(property);

            theValues.HasChild("test").ShouldBeTrue();
        }

        [Test]
        public void has_child_negative_no_properties()
        {
            var property = new PropertyBlock("test") { Block = new ObjectBlock() };
            theBlock.AddProperty(property);

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
            var property = new PropertyBlock("test") { Block = new ObjectBlock() };
            property.Block.AddProperty(new PropertyBlock("sub-property"));
            theBlock.AddProperty(property);

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
            block1.AddProperty(new PropertyBlock("Url") { Block = new ObjectBlock { Value = "url1"}});

            var block2 = new ObjectBlock();
            block2.AddProperty(new PropertyBlock("Url") { Block = new ObjectBlock { Value = "url2" } });

            var property = new PropertyBlock("feeds");
            property.AddBlock(block1);
            property.AddBlock(block2);
            theBlock.AddProperty(property);

            var values = theValues.GetChildren("Feeds").ToArray();

            values[0].Get("Url").ShouldEqual("url1");
            values[1].Get("Url").ShouldEqual("url2");
        }

        [Test]
        public void get_mapped_children_for_existing_values()
        {
            var block1 = new ObjectBlock();
            block1.AddProperty(new PropertyBlock("Url") { Block = new ObjectBlock { Value = "url1" } });

            var block2 = new ObjectBlock();
            block2.AddProperty(new PropertyBlock("Url") { Block = new ObjectBlock { Value = "url2" } });

            var property = new PropertyBlock("feed");
            property.AddBlock(block1);
            property.AddBlock(block2);
            theBlock.AddProperty(property);

            var mappedValueSource = new ObjectBlockValues<Solution>(theBlock, new FeedObjectSettings());

            var values = mappedValueSource.GetChildren("Feeds").ToArray();

            values[0].Get("Url").ShouldEqual("url1");
            values[1].Get("Url").ShouldEqual("url2");
        }

        [Test]
        public void get_implicit_value()
        {
            var property = new PropertyBlock("feed");
            property.Block = new ObjectBlock { Value = "http://www.google.com" };

            theBlock.AddProperty(property);

            var mappedValueSource = new ObjectBlockValues<FeedObject>(property.Block, new FeedObjectSettings());

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

            public Accessor ImplicitValue(Type type, ObjectBlock block, string key)
            {
                return ReflectionHelper.GetAccessor<FeedObject>(x => x.Url);
            }

            public Accessor FindImplicitValue(Type type, ObjectBlock block)
            {
                throw new NotImplementedException();
            }

            public Type FindCollectionType(Type type, string key)
            {
                return type;
            }
        }
    }
}