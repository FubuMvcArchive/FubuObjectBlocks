using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Configuration;

namespace FubuObjectBlocks.Settings
{
    public class ObjectBlockSettingsProvider : ISettingsProvider
    {
        private readonly Lazy<ObjectBlockCollection> _collection;
        private readonly IObjectBlockReader _reader;

        public ObjectBlockSettingsProvider(IEnumerable<IObjectBlockSource> sources, IObjectBlockReader reader)
        {
            _reader = reader;
            _collection = new Lazy<ObjectBlockCollection>(() => new ObjectBlockCollection(sources.SelectMany(x => x.Blocks())));
        }

        public ObjectBlockCollection Blocks { get { return _collection.Value; } }

        public T SettingsFor<T>() where T : class, new()
        {
            var value = SettingsFor(typeof (T)) as T;
            return value ?? new T();
        }

        public object SettingsFor(Type settingsType)
        {
            var name = settingsType.Name;
            if (!Blocks.Has(name))
            {
                return Activator.CreateInstance(settingsType);
            }

            var block = Blocks.Find(name);
            return _reader.Read(settingsType, block);
        }
    }
}