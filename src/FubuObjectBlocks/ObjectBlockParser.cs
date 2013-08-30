using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FubuCore;

namespace FubuObjectBlocks
{

    public class MonadicBlockParser : IObjectBlockParser
    {
        public Regex ObjectBlock = new Regex(@"(\s*)(\w+):$");
        public Regex PropertyBlock = new Regex(@"(\s*)(\w+):$");

        public ObjectBlock Parse(string input)
        {
            return input
                .Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(new Accumulator(new ObjectBlock()), (acc, line) =>
                {
                    var match = ObjectBlock.Match(line);
                    if (match.Success)
                    {
                        var indent = match.Groups[1].Value;
                        if (acc.Indent == Indent(indent))
                        {
                            
                        }

                        var name = match.Groups[2].Value;
                        var block = new ObjectBlock(name)
                        {
                        };

                        return acc.Nest(block);
                    }

                    var match2 = PropertyBlock.Match(line);
                    {
                        if(match.Success)
                        {
                            return acc.AddBlock(new PropertyBlock(""));
                        }
                    }

                    return acc;
                })
                .Root;
        }

        private int Indent(string whitespace)
        {
            return 0;
        }
    }

    public class Accumulator
    {
        public int Indent { get; set; }

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
            Indent = Indent + 1;
            CurrentObject.AddBlock(block);
            _objectBlocks.Push(block);
            return this;
        }

        public void EndIndent()
        {
            Indent = Indent - 1;
            _objectBlocks.Pop();
        }
    }

}