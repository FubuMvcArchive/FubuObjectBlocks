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
        private readonly ObjectBlockSerializer _serializer;

        public ParsingScenario(string fileName)
        {
            _fileName = fileName;
            _files = new FileSystem();

            _parser = new ObjectBlockParser();

            var formatter = new DisplayFormatter(new InMemoryServiceLocator(), new Stringifier());
            _serializer = new ObjectBlockSerializer(_parser, ObjectResolver.Basic(), new TypeDescriptorCache(), formatter);
        }

        public string FileName { get { return _fileName; } }

        private string readFile()
        {
            return _files.ReadStringFromFile(_fileName);
        }

        public ObjectBlock Read()
        {
            return _parser.Parse(readFile());
        }

        public T Read<T>()
        {
            return _serializer.Deserialize<T>(readFile());
        }

        public T Read<T, TMap>()
            where TMap : ObjectBlockSettings<T>, new()
        {
            return _serializer.Deserialize<T, TMap>(readFile());
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