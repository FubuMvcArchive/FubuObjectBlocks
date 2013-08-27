using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests
{
    [TestFixture]
    public class block_for_simple_object
    {
        [Test]
        public void sets_the_properties()
        {
            var target = new SimpleTarget {Key = "test", Value = "value"};
            var serializer = ObjectBlockSerializer.Basic();

            var block = serializer.BlockFor(target, new ObjectBlockSettings());
            
            block.FindProperty("Key").Name.ShouldEqual("key");
            block.FindProperty("Key").Block.Value.ShouldEqual("test");

            block.FindProperty("Value").Name.ShouldEqual("value");
            block.FindProperty("Value").Block.Value.ShouldEqual("value");
        }

        public class SimpleTarget
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}