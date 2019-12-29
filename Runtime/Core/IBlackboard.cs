namespace EntitiesBT.Core
{
    public interface IBlackboard
    {
        object this[object key] { get; set; }
    }

    public static class BlackboardExtensions
    {
        public static T GetData<T>(this IBlackboard bb)
        {
            return (T) bb[typeof(T)];
        }

        public static void SetData<T>(this IBlackboard bb, T data)
        {
            bb[typeof(T)] = data;
        }
    }
}
