using System.Text.RegularExpressions;

namespace FubuObjectBlocks
{
    public class NestedObjectBlockParser : RegexParser
    {
        public static readonly Regex NestedObjectBlockRegex = new Regex(@"^(\w+):$", RegexOptions.Compiled);

        public override Regex Regex
        {
            get { return NestedObjectBlockRegex; }
        }

        public override BlockAccumulator Include(BlockAccumulator acc, IBlock block)
        {
            return acc.Nest(block as ObjectBlock);
        }

        public override IBlock MakeBlock(Match match)
        {
            var name = match.Groups[1].Value;
            return new ObjectBlock(name);
        }
    }
}