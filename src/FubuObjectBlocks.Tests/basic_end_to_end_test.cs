using System.Collections.Generic;
using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests
{
    [TestFixture]
    public class basic_end_to_end_test
    {
        private ParsingScenario theScenario;
        private EndToEndTarget theTarget;

        [SetUp]
        public void SetUp()
        {
            theScenario = ParsingScenario.Create(scenario =>
            {
                scenario.WriteLine("name: 'Joel'");
                scenario.WriteLine("email: 'joel@arnold.com'");
                scenario.WriteLine("");

                scenario.WriteLine("target:");
                scenario.WriteLine("  url: '/test'");
                scenario.WriteLine("");

                scenario.WriteLine("item 'value1', property: 'test'");
                scenario.WriteLine("item 'value2', property: 'testing'");
                scenario.WriteLine("");

                scenario.WriteLine("feed 'http://localhost', mode: 'fixed'");
            });

            theTarget = theScenario.Read<EndToEndTarget, EndToEndSettings>();
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Dispose();
        }

        [Test]
        public void smoke()
        {
            theTarget.Name.ShouldEqual("Joel");
            theTarget.Email.ShouldEqual("joel@arnold.com");

            theTarget.Target.Url.ShouldEqual("/test");

            var values = theTarget.Items.ToArray();
            values[0].Value.ShouldEqual("value1");
            values[0].Property.ShouldEqual("test");

            values[1].Value.ShouldEqual("value2");
            values[1].Property.ShouldEqual("testing");

            var feeds = theTarget.Feeds.ToArray();
            feeds[0].Url.ShouldEqual("http://localhost");
            feeds[0].Mode.ShouldEqual("fixed");
        }
    }

    public class EndToEndSettings : ObjectBlockSettings<EndToEndTarget>
    {
        public EndToEndSettings()
        {
            Collection(x => x.Items)
                .ExpressAs("item")
                .ImplicitValue(x => x.Value);
        }
    }

    public class EndToEndTarget
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public NestedTarget Target { get; set; }

        public IEnumerable<NestedItem> Items { get; set; }

        [BlockSettings(ExpressAs = "feed", ImplicitProperty = "url")]
        public IEnumerable<FeedObject> Feeds { get; set; } 
    }

    public class NestedTarget
    {
        public string Url { get; set; }
    }

    public class NestedItem
    {
        public string Value { get; set; }
        public string Property { get; set; }
    }

    public class FeedObject
    {
        public string Url { get; set; }
        public string Mode { get; set; }
    }
}