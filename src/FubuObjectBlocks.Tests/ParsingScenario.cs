using System;
using System.IO;
using System.Text;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Formatting;
using FubuCore.Reflection;

namespace FubuObjectBlocks.Tests
{
    public class ParsingScenario : IDisposable
    {
        private readonly string _fileName;
        private readonly IFileSystem _files;
        private readonly IObjectBlockParser _parser;
        private readonly ObjectBlockReader _reader;
        private readonly BlockRegistry _blockRegistry;

        public ParsingScenario(string fileName)
        {
            _fileName = fileName;
            _files = new FileSystem();

            _parser = new ObjectBlockParser();

            _blockRegistry = BlockRegistry.Basic();
            _reader = new ObjectBlockReader(_parser, ObjectResolver.Basic(), _blockRegistry);
        }

        public string FileName { get { return _fileName; } }

        private string readFile()
        {
            return _files.ReadStringFromFile(_fileName);
        }

        public ObjectBlock Read()
        {
            return _parser.Parse(readFile(), new ObjectBlockSettings());
        }

        public T Read<T>()
        {
            return _reader.Read<T>(readFile());
        }

        public T Read<T, TMap>()
            where TMap : ObjectBlockSettings<T>, new()
        {
            _blockRegistry.RegisterSettings<TMap>();
            return _reader.Read<T>(readFile());
        }

        public static ParsingScenario Create(Action<ScenarioDefinition> configure)
        {
            var definition = new ScenarioDefinition();
            configure(definition);

            return definition.As<IScenarioBuilder>().Build();
        }

        public interface IScenarioBuilder
        {
            ParsingScenario Build();
        }

        public class ScenarioDefinition : IScenarioBuilder
        {
            private readonly string _fileName;
            private readonly StringBuilder _contents;

            public ScenarioDefinition()
            {
                _fileName = Guid.NewGuid().ToString("N") + ".fubu";
                _contents = new StringBuilder();
            }

            public ScenarioDefinition WriteLine(string line)
            {
                _contents.AppendLine(line);
                return this;
            }

            ParsingScenario IScenarioBuilder.Build()
            {
                using (var writer = new StreamWriter(_fileName))
                {
                    writer.Write(_contents.ToString());
                }

                return new ParsingScenario(_fileName);
            }
        }

        public void Dispose()
        {
            _files.DeleteFile(_fileName);
        }
    }
}