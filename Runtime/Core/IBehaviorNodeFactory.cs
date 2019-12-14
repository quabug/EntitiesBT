namespace EntitiesBT.Core
{
    public interface IBehaviorNodeFactory
    {
        IBehaviorNode Create(int nodeType);
    }
}
