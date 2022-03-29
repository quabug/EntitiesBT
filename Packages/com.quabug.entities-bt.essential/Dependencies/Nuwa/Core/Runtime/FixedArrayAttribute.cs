using System.Collections.Generic;
using JetBrains.Annotations;

namespace Nuwa
{
    [BaseTypeRequired(typeof(IList<>))]
    public class FixedArrayAttribute : MultiPropertyAttribute
    {
        public int FixedLength { get; }
        public string GetLength { get; }

        public FixedArrayAttribute(int fixedLength) => FixedLength = fixedLength;
        public FixedArrayAttribute(string getLength) => GetLength = getLength;
    }
}