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


        private CollectionItemBlock theCollection
        {
            get
            {
                var root = theScenario.Read();
                return root.Blocks.OfType<CollectionItemBlock>().Single();
            }
        }

        [Test]
        public void reads_the_property_name()
        {
            theCollection.Name.ShouldEqual("feed");
        }

        [Test]
        public void reads_the_multiple_values()
        {
            var collectionItems = theCollection.Blocks.ToArray();

            collectionItems[0].Value.ShouldEqual("url1");
            collectionItems[0].FindProperty("mode").Value.ShouldEqual("fixed");

            collectionItems[1].Value.ShouldEqual("url2");
            collectionItems[1].FindProperty("mode").Value.ShouldEqual("float");
        }
    }
}