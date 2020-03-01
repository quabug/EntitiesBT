using System;

namespace EntitiesBT.Variable
{
    [AttributeUsage(AttributeTargets.Class)]
    public class VariableAttribute : Attribute
    {
        public Guid Guid { get; } 
        public int Id { get; }
        public string GetDataFunc { get; }
        public string GetDataRefFunc { get; }
        
        public VariableAttribute(string guid, string getDataFunc = "GetData", string getDataRefFunc = "GetDataRef")
        {
            Guid = Guid.Parse(guid);
            Id = guid.GetHashCode();
            GetDataFunc = getDataFunc;
            GetDataRefFunc = getDataRefFunc;
        }
    }
}
