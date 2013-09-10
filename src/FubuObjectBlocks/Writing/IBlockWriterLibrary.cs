using System.Collections.Generic;
using System.Linq;

namespace FubuObjectBlocks.Writing
{
    public interface IBlockWriterLibrary
    {
        IBlockWriter WriterFor(BlockWritingContext context);
    }

    public class BlockWriterLibrary : IBlockWriterLibrary
    {
        private readonly IEnumerable<IBlockWriter> _writers;

        public BlockWriterLibrary(IEnumerable<IBlockWriter> writers)
        {
            _writers = writers;
        }

        public IEnumerable<IBlockWriter> AllWriters()
        {
            yield return new PropertyBlockWriter();
            yield return new CollectionBlockWriter();
            yield return new ImplicitValueBlockWriter();
            
            foreach (var writer in _writers)
            {
                yield return writer;
            }

            yield return new NestedBlockWriter();
        }

        public IBlockWriter WriterFor(BlockWritingContext context)
        {
            return AllWriters().FirstOrDefault(x => x.Matches(context));
        }
        
        public static BlockWriterLibrary Basic()
        {
            return new BlockWriterLibrary(new IBlockWriter[0]);
        }
    }
}