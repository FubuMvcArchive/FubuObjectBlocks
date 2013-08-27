using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests
{
    [TestFixture]
    public class parse_inline_blocks_as_a_collection
    {
        private ParsingScenario theScenario;

        [SetUp]
        public void SetUp()
        {
            theScenario = ParsingScenario.Create(scenario =>
            {
                scenario.WriteLine("feed 'url1', mode: 'fixed'");
                scenario.WriteLine("feed 'url2', mode: 'float'");
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
        public void reads_the_property_name()
        {
            var property = theProperty;
            property.Name.ShouldEqual("feed");
        }

        [Test]
        public void reads_the_multiple_values()
        {
            var values = theProperty.Blocks.ToArray();

            values[0].Value.ShouldEqual("url1");
            values[0].FindProperty("mode").Block.Value.ShouldEqual("fixed");

            values[1].Value.ShouldEqual("url2");
            values[1].FindProperty("mode").Block.Value.ShouldEqual("float");
        }
    }
}