using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FubuCore;

namespace FubuObjectBlocks
{
    public interface IBlock
    {
        string ToString(int indent = 0);
        string Name { get; }
    }

    public class ObjectBlock : IBlock
    {
        private readonly IList<IBlock> _blocks = new List<IBlock>();

        public IEnumerable<IBlock> Blocks { get { return _blocks; } }

        public ObjectBlock()
        {
        }

        public ObjectBlock(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public void AddBlock(IBlock block)
        {
            _blocks.Add(block);
        }

        public bool Has(string name)
        {
            return FindBlock(name) != null;
        }

        public IBlock FindBlock(string name)
        {
            return _blocks.SingleOrDefault(x => x.Name.EqualsIgnoreCase(name));
        }

        //use tostring instead
        public string Write()
        {
            return ToString();
        }

        //        private static void write(ObjectBlock block, StringBuilder builder, int depth = 0, int indent = 0)
//        {
//            block.Properties.Each(property =>
//            {
//                if (property.Block.Value.IsNotEmpty())
//                {
//                    var indentation = "";
//                    //NOTE: all unit tests passing with this commented out, seems its not needed?
//                    //when depth is 0 so is indent
//                    if (depth != 0)
//                    {
//                        for (var i = 0; i < indent * 2; i++)
//                        {
//                            indentation += " ";
//                        }
//                    }
//
//                    if (property.Blocks.Count() > 1)
//                    {
//                        property.Blocks.Each(x =>
//                        {
//                            builder.AppendLine();
//                            builder.Append("{0}{1} '{2}'".ToFormat(indentation, property.Name, x.Value));
//                            x.Properties.Each(p =>
//                            {
//                                builder.Append(", ");
//                                builder.Append("{0}: '{1}'".ToFormat(p.Name, p.Block.Value));
//                            });
//                        });
//                    }
//                    else {builder.Append("{0}{1} '{2}'".ToFormat(indentation, property.Name, property.Block.Value));}
//
//
//                    builder.AppendLine();
//                }
//                else
//                {
//                    var indentation = "";
//                    for (var i = 0; i < indent * 2; i++)
//                    {
//                        indentation += " ";
//                    }
//
//                    builder.AppendLine("{0}{1}:".ToFormat(indentation, property.Name));
//                    write(property.Block, builder, depth + 1, indent + 1);
//                }
//            });
//        }

        public string ToString(int indent = 0)
        {
            return new[] { BlockIndenter.Indent("{0}:".ToFormat(Name), indent) }
                .Concat(Blocks.Select(x => x.ToString(indent)))
                .Join(Environment.NewLine);
        }
    }

    public static class BlockIndenter
    {
        public static int IndentSize;

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