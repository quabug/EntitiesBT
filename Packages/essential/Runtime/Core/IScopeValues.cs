using System;

namespace EntitiesBT.Core
{
    public interface IScopeValues
    {
        IntPtr ValuePtr { get; }
        int Size { get; }
        int Offset { get; set; }
    }
}