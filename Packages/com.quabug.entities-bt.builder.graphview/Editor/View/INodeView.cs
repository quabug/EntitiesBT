using System;

namespace EntitiesBT.Editor
{
    public interface INodeView : IDisposable
    {
        void SyncPosition();
    }
}