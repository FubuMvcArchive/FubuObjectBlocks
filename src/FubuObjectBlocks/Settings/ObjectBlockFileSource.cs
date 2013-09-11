using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuObjectBlocks.Settings
{
    public class ObjectBlockFileSource : IObjectBlockSource
    {
        private readonly ObjectBlockFileSettings _settings;
        private readonly IFileSystem _fileSystem;
        private readonly IObjectBlockReader _reader;

        public ObjectBlockFileSource(ObjectBlockFileSettings settings, IFileSystem fileSystem, IObjectBlockReader reader)
        {
            _settings = settings;
            _fileSystem = fileSystem;
            _reader = reader;
        }

        public IEnumerable<ObjectBlock> Blocks()
        {
            return _settings
                .Files
                .Where(x => _fileSystem.FileExists(x))
                .SelectMany(file =>
                {
                    var contents = _fileSystem.ReadStringFromFile(file);
                    return _reader.Read(contents).Blocks.OfType<ObjectBlock>();
                });
        }
    }
}