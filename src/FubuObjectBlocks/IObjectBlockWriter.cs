namespace FubuObjectBlocks
{
    public interface IObjectBlockWriter
    {
        string Write(object input);
        ObjectBlock BlockFor(object input, string objectName = null);
    }
}