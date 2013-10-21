using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
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
        private IBlockSorter _sorter;

        public BlockRegistry()
            : this(new IBlockNamingStrategy[0])
        {
        }

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

        public void RegisterSettings(Type type, IObjectBlockSettings settings)
        {
            _settings.Fill(type, settings);
        }

        public void RegisterSettings<T>()
            where T : IObjectBlockSettings, new()
        {
            var settingsType = typeof (T);
            if (!settingsType.Closes(typeof (ObjectBlockSettings<>)))
            {
                throw new InvalidOperationException("Must subclass from ObjectBlockSettings<T>");
            }
            
            var type = settingsType.BaseType.GetGenericArguments()[0];
            var settings = new T().As<ObjectBlockSettings>();
            settings.Include(type);

            RegisterSettings(type, settings);
        }

        public IBlockSorter Sorter
        {
            get { return _sorter ?? (_sorter = new BlockSorter()); }
            set { _sorter = value; }
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