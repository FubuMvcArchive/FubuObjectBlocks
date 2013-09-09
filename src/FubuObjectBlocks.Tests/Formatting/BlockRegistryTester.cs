using FubuObjectBlocks.Formatting;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuObjectBlocks.Tests.Formatting
{
    [TestFixture]
    public class BlockRegistryTester
    {
        [Test]
        public void custom_naming_strategy()
        {
            var name = new BlockName("test");

            var strategy = MockRepository.GenerateStub<IBlockNamingStrategy>();
            strategy.Stub(x => x.Matches(name)).Return(true);

            var registry = new BlockRegistry(new[] {strategy});
            registry.NamingStrategyFor(name).ShouldBeTheSameAs(strategy);
        }

        [Test]
        public void default_naming_strategy()
        {
            var name = new BlockName("test");

            var strategy = MockRepository.GenerateStub<IBlockNamingStrategy>();
            strategy.Stub(x => x.Matches(name)).Return(false);

            var registry = new BlockRegistry(new[] { strategy });
            registry.NamingStrategyFor(name).ShouldBeOfType<DefaultBlockNamingStrategy>();
        }
    }
}