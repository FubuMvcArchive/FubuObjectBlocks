using FubuCore.Binding.Values;

namespace FubuObjectBlocks
{
    public interface IObjectValueBuilder
    {
        IValueSource Build(ObjectBlock block, IObjectBlockSettings settings);
    }

    public class ObjectValueBuilder<T> : IObjectValueBuilder
    {
        public IValueSource Build(ObjectBlock block, IObjectBlockSettings settings)
        {
            return new ObjectBlockValues<T>(block, settings);
        }
    }
}