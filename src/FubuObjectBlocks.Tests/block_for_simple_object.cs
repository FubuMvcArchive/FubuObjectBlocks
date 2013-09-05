using System;
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

            Action<string,string> assert = (name,value) =>
            {
                var result = block.FindProperty(name);
                result.ShouldNotBeNull();
                result.Name.ShouldEqual(name.FirstLetterLowercase());
                result.Value.ShouldEqual(value);
            };

            assert("Key", "test");
            assert("Value", "value");
        }

        public class SimpleTarget
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}