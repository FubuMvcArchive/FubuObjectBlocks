using System.Linq;
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
                scenario.WriteLine("  property1: 'string value'");
                scenario.WriteLine("  property2: 'another string value'");
                scenario.WriteLine("  nestedObject:");
                scenario.WriteLine("    nestedProperty1: '1'");
                scenario.WriteLine("    nestedProperty2: '2'");
            });
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Dispose();
        }

        private ObjectBlock theBlock { get { return theScenario.Read().Blocks.Single() as ObjectBlock; } }

        [Test]
        public void reads_the_object_block()
        {
            var block = theBlock;
            block.Name.ShouldEqual("blockProperty");
        }

        [Test]
        public void reads_the_nested_object_block()
        {
            var nestedObject = theBlock.GetBlocks<ObjectBlock>().Single();
            nestedObject.Name.ShouldEqual("nestedObject");

            var nestedProperties = nestedObject.GetBlocks<PropertyBlock>().ToArray();

            nestedProperties[0].Name.ShouldEqual("nestedProperty1");
            nestedProperties[0].Value.ShouldEqual("1");

            nestedProperties[1].Name.ShouldEqual("nestedProperty2");
            nestedProperties[1].Value.ShouldEqual("2");
        }
    }
}