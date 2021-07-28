using static System.Int32;

namespace EntitiesBT.Attributes
{
    public class HideIfAttribute : MultiPropertyAttribute
    {
        public bool LeaveEmptySpace { get; }
        public string ConditionFieldName { get; }
        public bool Value { get; }

        public HideIfAttribute(string conditionFieldName, bool value = true, bool leaveEmptySpace = false)
        {
            order = MinValue;
            LeaveEmptySpace = leaveEmptySpace;
            ConditionFieldName = conditionFieldName;
            Value = value;
        }
    }
}