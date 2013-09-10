using FubuObjectBlocks.Formatting;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests.Formatting
{
    [TestFixture]
    public class DefaultBlockNamingStrategyTester
    {
        private DefaultBlockNamingStrategy theStrategy { get { return new DefaultBlockNamingStrategy(); } }

        [Test]
        public void matches_non_null()
        {
            theStrategy.Matches(new BlockToken("test")).ShouldBeTrue();
        }

        [Test]
        public void no_match_for_empty()
        {
            theStrategy.Matches(BlockToken.Empty).ShouldBeFalse();
        }

        [Test]
        public void formats_camel_case_single_word()
        {
            theStrategy.NameFor(new BlockToken("Test")).ShouldEqual("test");
        }

        [Test]
        public void formats_camel_case_multi_word()
        {
            theStrategy.NameFor(new BlockToken("TestProperty")).ShouldEqual("testProperty");
        }
    }
}