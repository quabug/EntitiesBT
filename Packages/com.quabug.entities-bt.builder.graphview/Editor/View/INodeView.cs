using System;

namespace EntitiesBT.Editor
{
    public interface INodeView : IDisposable
    {
        int Id { get; }
        void SyncPosition();
    }
}