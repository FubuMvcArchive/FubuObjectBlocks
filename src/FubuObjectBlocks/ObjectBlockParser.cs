using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuObjectBlocks
{
    public class ObjectBlockParser : IObjectBlockParser
    {
        private readonly Stack<ObjectBlock> _blocks = new Stack<ObjectBlock>();
        private readonly Stack<IParsingMode> _modes = new Stack<IParsingMode>();
        private readonly IList<char> _characters = new List<char>(); 

        public ObjectBlockParser()
        {
            push(new Searching());
        }

        public ObjectBlock Parse(string input)
        {
            _blocks.Clear();

            _blocks.Push(new ObjectBlock());
            input.ReadLines(processLine);

            return _blocks.Last();
        }

        private bool empty { get { return !_characters.Any(); } }
        private IParsingMode mode { get { return _modes.Peek(); } }
        private ObjectBlock current { get { return _blocks.Peek(); } }

        private void reset()
        {
            while (_blocks.Count > 1)
            {
                _blocks.Pop();
            }

            _modes.Clear();
            _modes.Push(new Searching());
        }

        private void pushBlock(ObjectBlock block)
        {
            _blocks.Push(block);
        }

        private void popBlock()
        {
            _blocks.Pop();
        }

        private void push(IParsingMode parser)
        {
            _modes.Push(parser);
        }

        private void pop()
        {
            if (_modes.Count != 0)
            {
                _modes.Pop();
            }
        }

        private void processLine(string line)
        {
            line = line.TrimStart();
            if (line.IsEmpty() || line.StartsWith("#"))
            {
                reset();
                return;
            }

            var characters = line.ToCharArray();
            characters.Each(x => mode.Read(this, x));

            mode.EndOfLine(this);
        }

        private void addCharacter(char c)
        {
            _characters.Add(c);
        }

        private void endToken(Action<string> action)
        {
            var token = new string(_characters.ToArray());
            _characters.Clear();

            action(token);
        }

        public interface IParsingMode
        {
            void Read(ObjectBlockParser parser, char c);
            void EndOfLine(ObjectBlockParser parser);
        }

        public class Searching : IParsingMode
        {
            public ObjectBlock Block { get; set; }

            public void Read(ObjectBlockParser parser, char c)
            {
                var isWhiteSpace = char.IsWhiteSpace(c) && !parser.empty;
                var isPropertyTerminator = isWhiteSpace || c == ':';

                if (isPropertyTerminator)
                {
                    if (isWhiteSpace || Block != null)
                    {
                        parser.push(new PropertyMode());
                    }
                    
                    parser.endToken(token =>
                    {
                        var block = new ObjectBlock();
                        var target = Block ?? parser.current;

                        if (target.Has(token))
                        {
                            var property = target.FindBlock(token);
                            property.AddBlock(block);
                        }
                        else
                        {
                            var property = new PropertyBlock(token) { Block = block };
                            target.AddBlock(property);
                        }
                        
                        
                        parser.pushBlock(block);
                    });
                    
                    return;
                }

                if (!char.IsWhiteSpace(c))
                {
                    parser.addCharacter(c);
                }
            }

            public void EndOfLine(ObjectBlockParser parser)
            {
                // no-op
            }
        }

        public class InsideSingleQuote : IParsingMode
        {
            public void Read(ObjectBlockParser parser, char c)
            {
                if (c == '\'')
                {
                    parser.endToken(token =>
                    {
                        parser.current.Value = token;
                    });

                    parser.popBlock();
                    parser.pop();
                    return;
                }

                parser.addCharacter(c);
            }

            public void EndOfLine(ObjectBlockParser parser)
            {
                // TODO -- throw syntax error
                throw new NotImplementedException();
            }
        }

        public class PropertyMode : IParsingMode
        {
            public void Read(ObjectBlockParser parser, char c)
            {
                if (c == '\'')
                {
                    parser.push(new InsideSingleQuote());
                }
                else if (c == ',')
                {
                    var property = parser.current.Properties.Last();
                    parser.push(new Searching
                    {
                        Block = property.Blocks.Last()
                    });
                }
            }

            public void EndOfLine(ObjectBlockParser parser)
            {
                parser.pop();
                var mode = parser.mode as Searching;
                if (mode != null && mode.Block != null)
                {
                    mode.Block = null;
                }
            }
        }
    }
}