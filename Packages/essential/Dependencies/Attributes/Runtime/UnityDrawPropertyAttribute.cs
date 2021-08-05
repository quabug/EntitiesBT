namespace EntitiesBT.Attributes
{
    public class UnityDrawPropertyAttribute : MultiPropertyAttribute
    {
        public UnityDrawPropertyAttribute() => order = int.MaxValue;
    }
}