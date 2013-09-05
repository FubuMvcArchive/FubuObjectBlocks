using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FubuObjectBlocks
{
    public class ObjectBlockParser : IObjectBlockParser
    {
        private readonly IEnumerable<IBlockParser> _blockParsers;
        public static Regex IndentRegex = new Regex(@"^(\s*)(\w.*)", RegexOptions.Compiled);

        public ObjectBlockParser(IEnumerable<IBlockParser> blockParsers)
        {
            _blockParsers = blockParsers;
        }

        public ObjectBlockParser() : this(
            new IBlockParser[] 
            {
                new NestedObjectBlockParser(),
                new PropertyBlockParser(),
                new OneLineBlockParser()
            })
        {
        }

        public ObjectBlock Parse(string input, IObjectBlockSettings settings)
        {
            var result = input.Split(Environment.NewLine)
                .Aggregate(new BlockAccumulator(new ObjectBlock()), (acc, line) =>
                {
                    var match = IndentRegex.Match(line);
                    var indent = match.Groups[1].Value;
                    var rest = match.Groups[2].Value;
                    var rank = BlockIndenter.MeasureIndent(indent);
                    var parser = _blockParsers.FirstOrDefault(x => x.IsMatch(rest));
                    return parser != null
                        ? parser.Include(acc.SetRank(rank), parser.Parse(rest))
                        : acc;
                })
                .Root
                .MakeCollections(settings);
            return result;
        }
    }
}