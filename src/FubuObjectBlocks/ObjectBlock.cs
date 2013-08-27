using System.Collections.Generic;
using System.Linq;
using System.Text;
using FubuCore;

namespace FubuObjectBlocks
{
    public class ObjectBlock
    {
        private readonly IList<PropertyBlock> _properties = new List<PropertyBlock>();

        public IEnumerable<PropertyBlock> Properties { get { return _properties; } }

        public void AddProperty(PropertyBlock property)
        {
            _properties.Add(property);
        }

        public bool Has(string name)
        {
            return FindProperty(name) != null;
        }

        public PropertyBlock FindProperty(string name)
        {
            return _properties.SingleOrDefault(x => x.Name.EqualsIgnoreCase(name));
        }

        public string Value { get; set; }

        public string Write()
        {
            var builder = new StringBuilder();
            write(this, builder);

            return builder.ToString();
        }

        private static void write(ObjectBlock block, StringBuilder builder, int depth = 0, int indent = 0)
        {
            block.Properties.Each(property =>
            {
                if (property.Block.Value.IsNotEmpty())
                {
                    var indentation = "";
                    if (depth != 0)
                    {
                        for (var i = 0; i < indent * 2; i++)
                        {
                            indentation += " ";
                        }
                    }

                    if (property.Blocks.Count() > 1)
                    {
                        property.Blocks.Each(x =>
                        {
                            builder.AppendLine();
                            builder.Append("{0}{1} '{2}'".ToFormat(indentation, property.Name, x.Value));
                            x.Properties.Each(p =>
                            {
                                builder.Append(", ");
                                builder.Append("{0}: '{1}'".ToFormat(p.Name, p.Block.Value));
                            });
                        });
                    }
                    else {builder.Append("{0}{1} '{2}'".ToFormat(indentation, property.Name, property.Block.Value));}


                    builder.AppendLine();
                }
                else
                {
                    var indentation = "";
                    for (var i = 0; i < indent * 2; i++)
                    {
                        indentation += " ";
                    }

                    builder.AppendLine("{0}{1}:".ToFormat(indentation, property.Name));
                    write(property.Block, builder, depth + 1, indent + 1);
                }
            });

        }
    }
}