using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests
{
    [TestFixture]
    public class ExplicitIgnorePolicyTester
    {
        private Accessor theAccessor;
        private ExplicitIgnorePolicy thePolicy;

        [SetUp]
        public void SetUp()
        {
            theAccessor = ReflectionHelper.GetAccessor<ExplicitTarget>(x => x.Property1);
            thePolicy = new ExplicitIgnorePolicy(theAccessor);
        }

        [Test]
        public void matches_the_configured_accessor()
        {
            thePolicy.Matches(theAccessor).ShouldBeTrue();
        }

        [Test]
        public void no_match_on_different_accessor()
        {
            var other = ReflectionHelper.GetAccessor<ExplicitTarget>(x => x.Property2);
            thePolicy.Matches(other).ShouldBeFalse();
        }

        [Test]
        public void always_ignores()
        {
            thePolicy.Ignore(null, null).ShouldBeTrue();
        }

        public class ExplicitTarget
        {
            public string Property1 { get; set; }
            public string Property2 { get; set; }
        }
    }
}