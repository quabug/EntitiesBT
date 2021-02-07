namespace EntitiesBT.Attributes
{
    public class DisableIfAttribute : MultiPropertyAttribute
    {
        public string ConditionFieldName { get; }
        public bool Value { get; }

        public DisableIfAttribute(string conditionFieldName, bool value = true)
        {
            ConditionFieldName = conditionFieldName;
            Value = value;
        }
    }
}