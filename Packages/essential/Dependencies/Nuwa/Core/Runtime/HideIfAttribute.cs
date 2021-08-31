using System;
using static System.Int32;

namespace Nuwa
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class HideIfAttribute : MultiPropertyAttribute
    {
        public bool LeaveEmptySpace { get; }
        public string ConditionName { get; }
        public bool Value { get; }

        public HideIfAttribute(string conditionName, bool value = true, bool leaveEmptySpace = false)
        {
            order = MinValue;
            LeaveEmptySpace = leaveEmptySpace;
            ConditionName = conditionName;
            Value = value;
        }
    }
}