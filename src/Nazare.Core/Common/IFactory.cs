namespace Nazare.Core.Common
{
    public interface IFactory<T>
    {
        T Create(string strategy);
    }
}
