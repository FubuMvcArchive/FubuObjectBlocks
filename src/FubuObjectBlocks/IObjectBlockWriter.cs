using FubuObjectBlocks.Writing;

namespace FubuObjectBlocks
{
    public interface IObjectBlockWriter
    {
        string Write(object input);
        ObjectBlock BlockFor(object input, string objectName = null);
        ObjectBlock BlockFor(object input, BlockWritingContext context, string objectName = null);
    }
}