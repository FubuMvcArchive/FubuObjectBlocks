using System.Text.RegularExpressions;

namespace FubuObjectBlocks
{
    public abstract class RegexParser : IBlockParser
    {
        public abstract Regex Regex { get; }

        public bool IsMatch(string input)
        {
            return Regex.IsMatch(input);
        }

        public IBlock Parse(string input)
        {
            if (!IsMatch(input)) return null;

            var match = Regex.Match(input);
            return MakeBlock(match);
        }

        public abstract BlockAccumulator Include(BlockAccumulator acc, IBlock block);
        public abstract IBlock MakeBlock(Match match);
    }
}