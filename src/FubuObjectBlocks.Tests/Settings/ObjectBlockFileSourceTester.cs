using System.Linq;
using FubuCore;
using FubuObjectBlocks.Settings;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests.Settings
{
    [TestFixture]
    public class ObjectBlockFileSourceTester
    {
        private ObjectBlockFileSettings theSettings;
        private ObjectBlockFileSource theSource;

        private ParsingScenario file1;
        private ParsingScenario file2;

        [SetUp]
        public void SetUp()
        {
            file1 = ParsingScenario.Create(scenario =>
            {
                scenario.WriteLine("MyFirstSettings:");
                scenario.WriteLine("  property1: 'string value'");
                scenario.WriteLine("  property2: 'another string value'");
                scenario.WriteLine("");

                scenario.WriteLine("MySecondSettings:");
                scenario.WriteLine("  anotherProperty1: 'some value'");
                scenario.WriteLine("  anotherProperty2: 'another value of some kind'");
            });

            file2 = ParsingScenario.Create(scenario =>
            {
                scenario.WriteLine("MyThirdSettings:");
                scenario.WriteLine("  someProperty: 'random value'");
            });

            theSettings = new ObjectBlockFileSettings();
            theSettings.AddFile(file1.FileName);
            theSettings.AddFile(file2.FileName);

            theSource = new ObjectBlockFileSource(theSettings, new FileSystem(), ObjectBlockReader.Basic());
        }

        [TearDown]
        public void TearDown()
        {
            file1.Dispose();
            file2.Dispose();
        }

        [Test]
        public void parses_the_files()
        {
            var parsedBlocks = theSource.Blocks().ToArray();

            parsedBlocks[0].Name.ShouldEqual("MyFirstSettings");
            parsedBlocks[1].Name.ShouldEqual("MySecondSettings");

            parsedBlocks[2].Name.ShouldEqual("MyThirdSettings");
        }
    }
}