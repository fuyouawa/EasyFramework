namespace EasyFramework
{
    public interface IBindableArray<T> : IBindableValue<T[]>
    {
        T this[int index] { get; set; }
    }

    public class BindableArray<T> : BindableValue<T[]>, IBindableArray<T>
    {
        public T this[int index]
        {
            get => Value[index];
            set => Value[index] = value;
        }
    }
}
