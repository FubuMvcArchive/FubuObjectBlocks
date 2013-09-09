using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Formatting;
using FubuCore.Reflection;

namespace FubuObjectBlocks.Writing
{
    public class BlockWritingContext
    {
        private readonly IServiceLocator _services;
        private readonly IObjectBlockWriter _writer;
        private readonly BlockRegistry _registry;
        private readonly BlockName _name;
        private readonly object _subject;

        public BlockWritingContext(IServiceLocator services, IObjectBlockWriter writer, BlockRegistry registry, BlockName name, object subject)
        {
            _services = services;
            _writer = writer;
            _registry = registry;
            _name = name;
            _subject = subject;
        }

        public Accessor Accessor
        {
            get { return _name.Accessor; }
        }

        public IDisplayFormatter Formatter
        {
            get { return Get<IDisplayFormatter>(); }
        }

        public BlockName Name
        {
            get { return _name; }
        }

        public BlockRegistry Registry
        {
            get { return _registry; }
        }

        public IObjectBlockWriter Writer
        {
            get { return _writer; }
        }

        public object RawValue
        {
            get { return Accessor.GetValue(_subject); }
        }

        public T Get<T>()
        {
            return _services.GetInstance<T>();
        }

        public string GetBlockName()
        {
            return _registry.NameFor(_name);
        }

        public bool MatchesAccessor(Predicate<Accessor> predicate)
        {
            return Accessor != null && predicate(Accessor);
        }

        public static BlockWritingContext ContextFor<T>(Expression<Func<T, object>> expression, object subject = null)
        {
            var accessor = expression.ToAccessor();
            var name = new BlockName(accessor);

            var services = new InMemoryServiceLocator();
            services.Add<IDisplayFormatter>(new DisplayFormatter(services, new Stringifier()));
            
            return new BlockWritingContext(services, ObjectBlockWriter.Basic(), BlockRegistry.Basic(), name, subject);
        }
    }
}