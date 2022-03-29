using System;

namespace Nuwa
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ShowIfAttribute : MultiPropertyAttribute
    {
        public bool LeaveEmptySpace { get; }
        public string ConditionName { get; }
        public bool Value { get; }

        public ShowIfAttribute(string conditionName, bool value = true, bool leaveEmptySpace = false)
        {
            order = int.MinValue;
            LeaveEmptySpace = leaveEmptySpace;
            ConditionName = conditionName;
            Value = value;
        }
    }
}