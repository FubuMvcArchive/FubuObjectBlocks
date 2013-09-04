using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests
{
    [TestFixture]
    public class parse_an_inline_object_block
    {
        private ParsingScenario theScenario;

        [SetUp]
        public void SetUp()
        {
            theScenario = ParsingScenario.Create(scenario =>
            {
                scenario.WriteLine("feed 'some url', mode: 'float', stability: 'released'");
            });
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Dispose();
        }

        private ObjectBlock theObject
        {
            get
            {
                var root = theScenario.Read();
                return root.Blocks.OfType<ObjectBlock>().Single();
            }
        }

        [Test]
        public void reads_the_object_name_and_implicit_value()
        {
            var block = theObject;
            block.Name.ShouldEqual("feed");
            block.ImplicitValue.Value.ShouldEqual("some url");
        }

        [Test]
        public void reads_the_nested_properties()
        {
            var properties = theObject.GetBlocks<PropertyBlock>().ToArray();

            properties[0].Name.ShouldEqual("mode");
            properties[0].Value.ShouldEqual("float");

            properties[1].Name.ShouldEqual("stability");
            properties[1].Value.ShouldEqual("released");
        }
    }
}