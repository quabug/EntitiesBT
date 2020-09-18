using System;
using Runtime;

[NodeSearcherItem("PrintEntity")]
[Serializable]
public struct PrintEntityNode : IConstantNode
{
    [PortDescription(Runtime.ValueType.Entity)]
    public InputDataPort Entity;

    public void Execute<TCtx>(TCtx ctx) where TCtx : IGraphInstance
    {
        UnityEngine.Debug.Log(ctx.ReadEntity(Entity));
    }
}
