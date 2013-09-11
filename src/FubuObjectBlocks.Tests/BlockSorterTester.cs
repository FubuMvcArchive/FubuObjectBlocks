using System.Linq;
using FubuCore;
using FubuObjectBlocks.Formatting;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests
{
    [TestFixture]
    public class BlockSorterTester
    {
        [Test]
        public void sort_a_simple_block_with_properties()
        {
            var block = new ObjectBlock("Test");
            block.AddBlock(new PropertyBlock("Prop3"));
            block.AddBlock(new PropertyBlock("Prop2"));
            block.AddBlock(new PropertyBlock("Prop4"));
            block.AddBlock(new PropertyBlock("Prop1"));

            block.Sort(new BlockSorter());

            var properties = block.Blocks.Cast<PropertyBlock>().ToArray();
            properties[0].Name.ShouldEqual("Prop1");
            properties[1].Name.ShouldEqual("Prop2");
            properties[2].Name.ShouldEqual("Prop3");
            properties[3].Name.ShouldEqual("Prop4");
        }

        [Test]
        public void sort_a_block_with_properties_and_nested_blocks()
        {
            var block = new ObjectBlock("Test");
            block.AddBlock(new PropertyBlock("Prop2"));
            block.AddBlock(new PropertyBlock("Prop1"));

            var nested2 = new ObjectBlock("Nested2");
            nested2.AddBlock(new PropertyBlock("Prop5"));
            nested2.AddBlock(new PropertyBlock("Prop3"));

            block.AddBlock(new ObjectBlock("Nested1"));
            block.AddBlock(nested2);

            block.Sort(new BlockSorter());

            var blocks = block.Blocks.ToArray();
            blocks[0].As<PropertyBlock>().Name.ShouldEqual("Prop1");
            blocks[1].As<PropertyBlock>().Name.ShouldEqual("Prop2");

            blocks[2].As<ObjectBlock>().Name.ShouldEqual("Nested1");

            var nestedProperties = blocks[3].As<ObjectBlock>().Blocks.OfType<PropertyBlock>().ToArray();
            nestedProperties[0].Name.ShouldEqual("Prop3");
            nestedProperties[1].Name.ShouldEqual("Prop5");
        }

        [Test]
        public void sort_a_block_with_properties_and_nested_blocks_and_collections()
        {
            var block = new ObjectBlock("Test");
            block.AddBlock(new PropertyBlock("Prop2"));
            block.AddBlock(new PropertyBlock("Prop1"));

            var nested2 = new ObjectBlock("Nested2");
            nested2.AddBlock(new PropertyBlock("Prop5"));
            nested2.AddBlock(new PropertyBlock("Prop3"));

            block.AddBlock(new ObjectBlock("Nested1"));
            block.AddBlock(nested2);

            block.AddBlock(new CollectionBlock("Collection3"));
            block.AddBlock(new CollectionBlock("Collection1"));

            block.Sort(new BlockSorter());

            var blocks = block.Blocks.ToArray();
            blocks[0].As<PropertyBlock>().Name.ShouldEqual("Prop1");
            blocks[1].As<PropertyBlock>().Name.ShouldEqual("Prop2");

            blocks[2].As<ObjectBlock>().Name.ShouldEqual("Nested1");

            var nestedProperties = blocks[3].As<ObjectBlock>().Blocks.OfType<PropertyBlock>().ToArray();
            nestedProperties[0].Name.ShouldEqual("Prop3");
            nestedProperties[1].Name.ShouldEqual("Prop5");

            blocks[4].Name.ShouldEqual("Collection1");
            blocks[5].Name.ShouldEqual("Collection3");
        }
    }
}