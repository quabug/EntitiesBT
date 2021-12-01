using System;

namespace Nuwa
{
    public class FieldNameAttribute : MultiPropertyAttribute
    {
        public Type DeclaringType;
        public Type FieldType;
        public string DeclaringTypeVariable;
        public string FieldTypeVariable;
    }
}