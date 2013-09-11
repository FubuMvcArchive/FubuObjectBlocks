using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests.Settings
{
    [TestFixture]
    public class settings_data_parsing_smoke_test
    {
        private ParsingScenario theScenario;

        [SetUp]
        public void SetUp()
        {
            theScenario = ParsingScenario.Create(scenario =>
            {
                scenario.WriteLine("MySettings:");
                scenario.WriteLine("  property1: 'string value'");
                scenario.WriteLine("  property2: 'another string value'");
                scenario.WriteLine("");
                scenario.WriteLine("MyOtherSettings:");
                scenario.WriteLine("  anotherProperty1: 'some value'");
                scenario.WriteLine("  anotherProperty2: 'another value of some kind'");
            });
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Dispose();
        }

        private ObjectBlock theBlock { get { return theScenario.Read(); } }

        [Test]
        public void parses_the_multiple_blocks()
        {
            var blocks = theBlock.Blocks.ToArray();

            blocks[0].Name.ShouldEqual("MySettings");
            blocks[1].Name.ShouldEqual("MyOtherSettings");
        }
    }
}