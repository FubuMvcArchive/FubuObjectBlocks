using System.Collections.Generic;

namespace FubuObjectBlocks.Settings
{
    public class ObjectBlockFileSettings
    {
        private readonly IList<string> _files = new List<string>();

        public IEnumerable<string> Files { get { return _files; } } 

        public void ClearFiles()
        {
            _files.Clear();
        }

        public void AddFile(string file)
        {
            _files.Fill(file);
        }
    }
}