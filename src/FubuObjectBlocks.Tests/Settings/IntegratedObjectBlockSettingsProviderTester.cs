using FubuCore;
using FubuObjectBlocks.Settings;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests.Settings
{
    [TestFixture]
    public class IntegratedObjectBlockSettingsProviderTester
    {
        private ObjectBlockFileSettings theFileSettings;
        private ObjectBlockFileSource theSource;
        private ParsingScenario theScenario;
        private ObjectBlockReader theReader;
        private ObjectBlockSettingsProvider theProvider;
        private MySettings theSettings;

        [SetUp]
        public void SetUp()
        {
            theScenario = ParsingScenario.Create(scenario =>
            {
                scenario.WriteLine("MySettings:");
                scenario.WriteLine("  property1: 'string value'");
                scenario.WriteLine("  property2: 'another string value'");
            });

            theFileSettings = new ObjectBlockFileSettings();
            theFileSettings.AddFile(theScenario.FileName);

            theReader = ObjectBlockReader.Basic();
            theSource = new ObjectBlockFileSource(theFileSettings, new FileSystem(), theReader);
            theProvider = new ObjectBlockSettingsProvider(new IObjectBlockSource[] { theSource }, theReader);

            theSettings = theProvider.SettingsFor<MySettings>();
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Dispose();
        }

        [Test]
        public void builds_up_the_settings()
        {
            theSettings.Property1.ShouldEqual("string value");
            theSettings.Property2.ShouldEqual("another string value");
        }

        [Test]
        public void request_for_a_undefined_settings_type_just_creates_a_default_instance()
        {
            var settings = theProvider.SettingsFor<UndefinedSettings>();
            settings.Property.ShouldBeNull();
        }

        public class UndefinedSettings
        {
            public string Property { get; set; }
        }

        public class MySettings
        {
            public string Property1 { get; set; }
            public string Property2 { get; set; }
        }
    }
}