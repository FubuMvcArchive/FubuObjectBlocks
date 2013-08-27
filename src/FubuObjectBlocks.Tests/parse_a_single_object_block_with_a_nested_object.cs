using System.Linq;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests
{
    [TestFixture]
    public class parse_a_single_object_block_with_a_nested_object
    {
        private ParsingScenario theScenario;

        [SetUp]
        public void SetUp()
        {
            theScenario = ParsingScenario.Create(scenario =>
            {
                scenario.WriteLine("blockProperty:");
                scenario.WriteLine("  property1 'string value'");
                scenario.WriteLine("  property2 'another string value'");
                scenario.WriteLine("  nestedProperty:");
                scenario.WriteLine("    nestedProperty1 '1'");
                scenario.WriteLine("    nestedProperty2 '2'");
            });
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Dispose();
        }

        private ObjectBlock theBlock { get { return theScenario.Read(); } }

        [Test]
        public void reads_the_block_property()
        {
            var property = theBlock.Properties.Single();
            property.Name.ShouldEqual("blockProperty");
            property.Block.ShouldBeOfType<ObjectBlock>();
        }

        [Test]
        public void reads_the_nested_property()
        {
            var block = theBlock
                .Properties
                .Single().Block.As<ObjectBlock>();

            var nestedProperty = block.Properties.ToArray()[2];
            nestedProperty.Name.ShouldEqual("nestedProperty");

            var properties = nestedProperty.Block.Properties.ToArray();

            properties[0].Name.ShouldEqual("nestedProperty1");
            properties[0].Block.Value.ShouldEqual("1");

            properties[1].Name.ShouldEqual("nestedProperty2");
            properties[1].Block.Value.ShouldEqual("2");
        }
    }
}