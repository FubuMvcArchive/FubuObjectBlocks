using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuObjectBlocks.Formatting;

namespace FubuObjectBlocks
{
    public class BlockRegistry
    {
        private static readonly IEnumerable<IBlockNamingStrategy> Strategies;

        static BlockRegistry()
        {
            Strategies = new IBlockNamingStrategy[]
            {
                new DefaultBlockNamingStrategy(),
                new EmptyBlockNamingStrategy()
            };
        }

        private readonly IList<IBlockNamingStrategy> _strategies = new List<IBlockNamingStrategy>();
        private readonly Cache<Type, IObjectBlockSettings> _settings = new Cache<Type, IObjectBlockSettings>();

        public BlockRegistry(IEnumerable<IBlockNamingStrategy> strategies)
        {
            _strategies.AddRange(strategies);

            _settings.OnMissing = type => new ObjectBlockSettings();
        }

        public IEnumerable<IBlockNamingStrategy> AllNamingStrategies()
        {
            foreach (var strategy in _strategies)
            {
                yield return strategy;
            }

            foreach (var strategy in Strategies)
            {
                yield return strategy;
            }
        }

        public IBlockNamingStrategy NamingStrategyFor(BlockToken token)
        {
            return AllNamingStrategies().FirstOrDefault(x => x.Matches(token));
        }

        public string NameFor(BlockToken token)
        {
            return NamingStrategyFor(token).NameFor(token);
        }

        public IObjectBlockSettings SettingsFor(Type type)
        {
            return _settings[type];
        }

        public static BlockRegistry Basic()
        {
            return new BlockRegistry(new IBlockNamingStrategy[0]);
        }
    }
}