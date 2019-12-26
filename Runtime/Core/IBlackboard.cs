namespace EntitiesBT.Core
{
    public interface IBlackboard
    {
        object this[object key] { get; set; }
    }
}
