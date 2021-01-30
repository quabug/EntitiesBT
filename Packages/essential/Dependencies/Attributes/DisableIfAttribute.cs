namespace EntitiesBT.Attributes
{
    public class DisableIfAttribute : MultiPropertyDecoratorAttribute
    {
        public string ConditionFieldName;
        public bool Value;

        public DisableIfAttribute(string conditionFieldName, bool value = true)
        {
            ConditionFieldName = conditionFieldName;
            Value = value;
        }
    }
}