using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuObjectBlocks
{
    public static class BlockIndenter
    {
        public static int IndentSize = 2;
        public static string SingleIndent = Times(IndentSize, " ");

        public static string Times(int size, string content)
        {
            return Enumerable.Range(0, size).Select(x => content).Join(string.Empty);
        }

        public static string Indent(string content, int indent = 0)
        {
            return "{0}{1}".ToFormat(Times(indent, SingleIndent), content);
        }

        public static int MeasureIndent(string whitespace)
        {
            var spaces = whitespace.Replace("\t", SingleIndent);
            return spaces.Length/IndentSize;
        }
    }
}