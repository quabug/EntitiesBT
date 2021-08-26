namespace Nuwa
{
    public class UnityDrawPropertyAttribute : MultiPropertyAttribute
    {
        public UnityDrawPropertyAttribute() => order = int.MaxValue;
    }
}