using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuObjectBlocks
{
    public static class BlockIndenter
    {
        public static int IndentSize = 2;

        public static string Repeat(int size, string content)
        {
            return Enumerable.Range(0, size).Select(x => content).Join(string.Empty);
        }

        public static string Indent(string content, int indent = 0)
        {
            var indentString = Repeat(indent, Repeat(IndentSize, " "));
            return "{0}{1}".ToFormat(indentString, content);
        }
    }
}