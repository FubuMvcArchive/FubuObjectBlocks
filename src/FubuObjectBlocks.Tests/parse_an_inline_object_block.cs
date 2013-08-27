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

        private PropertyBlock theProperty
        {
            get
            {
                var root = theScenario.Read();
                return root.Properties.Single();
            }
        }

        [Test]
        public void reads_the_property_name_and_value()
        {
            var property = theProperty;
            property.Name.ShouldEqual("feed");
            property.Block.Value.ShouldEqual("some url");
        }

        [Test]
        public void reads_the_nested_properties()
        {
            var properties = theProperty.Block.Properties.ToArray();

            properties[0].Name.ShouldEqual("mode");
            properties[0].Block.Value.ShouldEqual("float");

            properties[1].Name.ShouldEqual("stability");
            properties[1].Block.Value.ShouldEqual("released");
        }
    }
}