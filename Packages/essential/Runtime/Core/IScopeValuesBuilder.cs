using System;

namespace EntitiesBT.Core
{
    public interface IScopeValuesBuilder
    {
        IntPtr ValuePtr { get; }
        int Size { get; }
        int Offset { get; set; }
    }
}