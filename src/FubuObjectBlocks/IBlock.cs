namespace FubuObjectBlocks
{
    public interface IBlock
    {
        string ToString(int indent = 0);
        string Name { get; }
    }
}