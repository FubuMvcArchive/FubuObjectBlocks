using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FubuCore;

namespace FubuObjectBlocks
{
    public interface IBlockParser
    {
        bool IsMatch(string input);
        IBlock BlockFor(string input);
        Accumulator Include(Accumulator acc, IBlock block);
    }

    public class PropertyBlockParser : RegexParser
    {
        private static readonly Regex PropertyBlockRegex = new Regex(@"^(\w+): '(.*?)'$", RegexOptions.Compiled);

        public override Regex Regex
        {
            get { return PropertyBlockRegex; }
        }

        public override Accumulator Include(Accumulator acc, IBlock block)
        {
            return acc.AddBlock(block);
        }

        public override IBlock MakeBlock(Match match)
        {
            var name = match.Groups[1].Value;
            var value = match.Groups[2].Value;
            return new PropertyBlock(name) {Value = value};
        }
    }

    public abstract class RegexParser : IBlockParser
    {
        public abstract Regex Regex { get; }

        public bool IsMatch(string input)
        {
            return Regex.IsMatch(input);
        }

        public IBlock BlockFor(string input)
        {
            if (!IsMatch(input)) return null;

            var match = Regex.Match(input);
            return MakeBlock(match);
        }

        public abstract Accumulator Include(Accumulator acc, IBlock block);
        public abstract IBlock MakeBlock(Match match);
    }

    public class NestedObjectBlockParser : RegexParser
    {
        public static readonly Regex NestedObjectBlockRegex = new Regex(@"^(\w+):$", RegexOptions.Compiled);

        public override Regex Regex
        {
            get { return NestedObjectBlockRegex; }
        }

        public override Accumulator Include(Accumulator acc, IBlock block)
        {
            return acc.Nest(block as ObjectBlock);
        }

        public override IBlock MakeBlock(Match match)
        {
            var name = match.Groups[1].Value;
            return new ObjectBlock(name);
        }
    }

    public class OneLineBlockParser : RegexParser
    {
        public static readonly Regex OneLineBlockRegex = new Regex(@"(\w+) '(.*?)'((?:, \w+: '.*?')*)");

        public override Regex Regex
        {
            get { return OneLineBlockRegex; }
        }

        public override Accumulator Include(Accumulator acc, IBlock block)
        {
            return acc.AddBlock(block);
        }

        public override IBlock MakeBlock(Match match)
        {
            var name = match.Groups[1].Value;
            var implicitValue = match.Groups[2].Value;

            var block = new ObjectBlock(name)
            {
                //TODO: how to get property name in here?
                ImplicitValue = new PropertyBlock("") {Value = implicitValue}
            };

            var otherProperties = match.Groups[3].Value;
            if (otherProperties.IsNotEmpty())
            {
                var propertyBlockParser = new PropertyBlockParser();
                var propertyBlocks = otherProperties.Split(", ")
                    .Select(propertyBlockParser.BlockFor)
                    .ToList();

                block.Blocks = propertyBlocks;
            }
            return block;
        }
    }

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

        public ObjectBlock Parse(string input)
        {
            var result = input.Split(Environment.NewLine)
                .Aggregate(new Accumulator(new ObjectBlock()), (acc, line) =>
                {
                    var match = IndentRegex.Match(line);
                    var indent = match.Groups[1].Value;
                    var rank = BlockIndenter.MeasureIndent(indent);
                    var rest = match.Groups[2].Value;

                    var parser = _blockParsers.FirstOrDefault(x => x.IsMatch(rest));
                    if (parser != null)
                    {
                        return parser.Include(acc.SetRank(rank), parser.BlockFor(rest));
                    }

                    return acc;
                })
                .Root;
            return result;
        }
    }

    public class Accumulator
    {
        public int Rank { get; set; }

        private readonly Stack<ObjectBlock> _objectBlocks;

        public Accumulator(ObjectBlock root)
        {
            _objectBlocks = new Stack<ObjectBlock>();
            _objectBlocks.Push(root);
        }

        public Accumulator AddBlock(IBlock block)
        {
            CurrentObject.AddBlock(block);
            return this;
        }

        private ObjectBlock CurrentObject
        {
            get { return _objectBlocks.Peek(); }
        }

        public ObjectBlock Root
        {
            get { return _objectBlocks.Reverse().First(); }
        }

        public Accumulator Nest(ObjectBlock block)
        {
            CurrentObject.AddBlock(block);
            _objectBlocks.Push(block);
            Rank = Rank + 1;
            return this;
        }

        public Accumulator SetRank(int rank)
        {
            while (Rank > rank)
            {
                _objectBlocks.Pop();
                Rank = Rank - 1;
            }
            return this;
        }
    }

}