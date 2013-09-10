using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        
        private readonly Stack<object> _objectStack = new Stack<object>();
        private readonly Stack<BlockToken> _tokenStack = new Stack<BlockToken>(); 
        

        public BlockWritingContext(IServiceLocator services, IObjectBlockWriter writer, BlockRegistry registry, object subject)
        {
            _services = services;
            _writer = writer;
            _registry = registry;

            _objectStack.Push(subject);
        }

        public Accessor Accessor
        {
            get { return Token != null ? Token.Accessor : null; }
        }

        public IDisplayFormatter Formatter
        {
            get { return Get<IDisplayFormatter>(); }
        }

        public BlockToken Token
        {
            get { return _tokenStack.Any() ? _tokenStack.Peek() : null; }
        }

        public object Subject
        {
            get { return _objectStack.Any() ? _objectStack.Peek() : null; }
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
            get { return Accessor.GetValue(Subject); }
        }

        public T Get<T>()
        {
            return _services.GetInstance<T>();
        }

        public string GetBlockName()
        {
            return _registry.NameFor(Token);
        }

        public bool MatchesAccessor(Predicate<Accessor> predicate)
        {
            return Accessor != null && predicate(Accessor);
        }

        public void StartProperty(PropertyInfo property)
        {
            _tokenStack.Push(new BlockToken(new SingleProperty(property)));
        }

        public void FinishProperty()
        {
            _tokenStack.Pop();
        }

        public void StartObject(object subject)
        {
            _objectStack.Push(subject);
        }

        public void FinishObject()
        {
            _objectStack.Pop();
        }

        public static BlockWritingContext ContextFor<T>(Expression<Func<T, object>> expression, object subject = null)
        {
            var property = ReflectionHelper.GetProperty(expression);

            var services = new InMemoryServiceLocator();
            services.Add<IDisplayFormatter>(new DisplayFormatter(services, new Stringifier()));
            
            var context =  new BlockWritingContext(services, ObjectBlockWriter.Basic(), BlockRegistry.Basic(), subject);
            context.StartProperty(property);

            return context;
        }
    }
}