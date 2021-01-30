namespace EntitiesBT.Attributes
{
    public class UnityDrawPropertyAttribute : MultiPropertyDecoratorAttribute
    {
        public UnityDrawPropertyAttribute() => Order = int.MaxValue;
    }
}