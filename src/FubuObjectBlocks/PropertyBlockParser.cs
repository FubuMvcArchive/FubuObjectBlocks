using System.Text.RegularExpressions;

namespace FubuObjectBlocks
{
    public class PropertyBlockParser : RegexParser
    {
        private static readonly Regex PropertyBlockRegex = new Regex(@"^(\w+): '(.*?)'$", RegexOptions.Compiled);

        public override Regex Regex
        {
            get { return PropertyBlockRegex; }
        }

        public override BlockAccumulator Include(BlockAccumulator acc, IBlock block)
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
}