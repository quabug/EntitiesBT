using System;

namespace EntitiesBT.Core
{
    public interface IScopeValuesBuilder
    {
        IntPtr ValuePtr { get; }
        int Size { get; }
        int Offset { get; set; }

        object GetPreviewValue(string path);
        void SetPreviewValue(string path, object value);
    }
}