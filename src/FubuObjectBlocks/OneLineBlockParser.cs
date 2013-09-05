using System.Linq;
using System.Text.RegularExpressions;
using FubuCore;

namespace FubuObjectBlocks
{
    public class OneLineBlockParser : RegexParser
    {
        public static readonly Regex OneLineBlockRegex = new Regex(@"(\w+) '(.*?)'((?:, \w+: '.*?')*)");

        private readonly PropertyBlockParser _propertyBlockParser;

        public OneLineBlockParser()
        {
            _propertyBlockParser = new PropertyBlockParser();
        }

        public override Regex Regex
        {
            get { return OneLineBlockRegex; }
        }

        public override BlockAccumulator Include(BlockAccumulator acc, IBlock block)
        {
            return acc.AddBlock(block);
        }

        public override IBlock MakeBlock(Match match)
        {
            var name = match.Groups[1].Value;
            var implicitValue = match.Groups[2].Value;

            var block = new ObjectBlock(name)
            {
                ImplicitValue = implicitValue
            };

            var otherProperties = match.Groups[3].Value;
            if (otherProperties.IsNotEmpty())
            {
                var propertyBlocks = otherProperties.Split(", ")
                    .Select(_propertyBlockParser.Parse)
                    .ToList();

                block.Blocks = propertyBlocks;
            }
            return block;
        }
    }
}