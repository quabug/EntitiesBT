```
_____      _   _ _   _           ______ _____ 
|  ___|    | | (_) | (_)          | ___ \_   _|
| |__ _ __ | |_ _| |_ _  ___  ___ | |_/ / | |  
|  __| '_ \| __| | __| |/ _ \/ __|| ___ \ | |  
| |__| | | | |_| | |_| |  __/\__ \| |_/ / | |  
\____/_| |_|\__|_|\__|_|\___||___/\____/  \_/  
                                               
```
Behavior Tree framework based on and used for Unity Entities (DOTS)

## Release Notes
- [1.2.0](https://github.com/quabug/EntitiesBT/pull/160#issue-696822887)

## Why another Behavior Tree framework?
Existing BT frameworks do not support Entities out of the box.

## Features
- Actions are easy to read/write data from/to entity.
- Use Component of Unity directly instead of own editor window to maximize compatibility of other plugins.
- Data-oriented design, save all nodes data into a continuous data blob ([NodeBlob.cs](Packages/essential/Runtime/Entities/NodeBlob.cs))
- Node has no internal states.
- Separate runtime nodes and editor nodes.
- Easy to extend.
- Also compatible with Unity GameObject without any entity.
- Able to serialize behavior tree into the binary file.
- Flexible thread control: force on the main thread, force on job thread, controlled by behavior tree.
- Runtime debug window to show the states of nodes.
- Optimized. 0 GC allocated by behavior tree itself after initialized, only 64Byte GC allocated every tick by [`CreateArchetypeChunkArrayAsync`](Packages/essential/Runtime/Entities/VirtualMachineSystem.cs#L59). 

## Disadvantages
- Incompatible with burst.
- Incompatible with il2cpp.
- Lack of action nodes. (Will add some actions as extensions if I need them)
- Not easy to modify tree structure at runtime.
- Node data must be compatible with `Blob` and created by [`BlobBuilder`](https://docs.unity3d.com/Packages/com.unity.entities@0.11/api/Unity.Entities.BlobBuilder.html)

## Packages
- [essential](Packages/essential): essential part of entities behavior tree, any extension should depend on this package.
- [codegen](Packages/codegen): automatically generate [entity query accesors](#entityquery) on the methods of nodes.
- [builder.component](Packages/builder.component): build behavior tree data from unity components.
- [builder.graphview](Packages/com.quabug.entities-bt.builder.graphview): build behavior tree data by graph with components.
- [builder.odin](Packages/builder.odin): advanced hierarchy builder based on Odin and its serializer.
- [builder.visual](Packages/builder.visual): build and use behavior tree by graph of DOTS visual scripting (suspended).
- [debug.component-viewer](Packages/debug.component-viewer): show selected entity with behavior tree as components in inspector of unity while running.
- [variable.scriptable-object](Packages/variable.scriptable-object): extension for using scriptable object data as variable source of behavior tree node.

## HowTo
### Installation
Requirement: Unity >= 2020.2 and entities package >= 0.14.0-preview.19

Install the packages either by

[UPM](https://docs.unity3d.com/Manual/upm-ui-giturl.html):
modify `Packages/manifest.json` as below
```
{
  "dependencies": {
    ...
    "com.quabug.entities-bt.builder.graphview": "1.4.0",
  },
  "scopedRegistries": [
    {
      "name": "package.openupm.com",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.quabug"
      ]
    }
  ]
}
```

or 

[OpenUPM](https://openupm.com/docs/getting-started.html#installing-an-upm-package):
```
openupm add com.quabug.entities-bt.builder.graphview
```

### Usage
#### GraphView Builder
##### Create behavior tree graph
<img width="600" alt="create graph" src="https://user-images.githubusercontent.com/683655/158053015-766eeff2-3263-4d3a-b51e-3eebf47c1a46.gif" />

##### Attach graph of behavior tree onto _Entity_
<img width="600" alt="attach graph" src="https://user-images.githubusercontent.com/683655/158052727-8a9376fc-c68b-4af1-bdaa-71473938c7aa.gif" />

#### Component Builder
##### Create behavior tree
<img width="600" alt="create" src="https://user-images.githubusercontent.com/683655/158053081-c17b41a0-ffae-48ee-955f-be92cdbf277d.gif" />

##### Attach behavior tree onto _Entity_
<img width="600" alt="attach" src="https://user-images.githubusercontent.com/683655/158053177-b7ee1081-4d81-4732-a4e8-f13f718ef58f.gif" />

#### Serialization
<img width="600" alt="save-to-file" src="https://user-images.githubusercontent.com/683655/72407209-b7b77900-3799-11ea-9de3-0703b1936f63.gif" />

#### Thread control
<img width="400" alt="thread-control" src="https://user-images.githubusercontent.com/683655/72407274-ee8d8f00-3799-11ea-9847-76ad6fdc5a37.png" />

- Force Run on Main Thread: running on the main thread only, will not use job to tick behavior tree. Safe to call `UnityEngine` method.
- Force Run on Job: running on job threads only, will not use the main thread to tick the behavior tree. Not safe to call `UnityEngine` method.
- Controlled by Behavior Tree: Running on job threads by default, but will switch to main thread once meet decorator of [`RunOnMainThread`](Packages/essential/Runtime/Nodes/Decorators/RunOnMainThreadNode.cs)
<img width="300" alt="" src="https://user-images.githubusercontent.com/683655/72407836-cdc63900-379b-11ea-8979-605e725ab0f7.png" />

#### Variant
##### Variant Types
- `BlobVariantReader`: read-only variant
- `BlobVariantWriter`: write-only variant
- `BlobVariantReaderAndWriter`: read-write variant, able to link to same source.

##### Variant Sources
- `LocalVariant`: regular variable, custom value will save into `NodeData`.

- `ComponentVariant`: fetch data from `Component` on `Entity`
  - _Component Value Name_: which value should be accessed from component
  - _Copy To Local Node_: Will read component data into a local node and never write back into component data. (Force `ReadOnly` access)

- `NodeVariant`: fetch data from the blob of another node
  - _Node Object_: another node should be accessed by this variable, and must be in the same behavior tree.
  - _Value Field Name_: the name of the data field in another node.
  - _Access Runtime Data_:
    - false: will copy data to local blob node while building, value change of _Node Object_ won't effect variable once build.
    - true: will access data field of _Node Object_ at runtime, something like reference value of _Node Object_.

- `ScriptableObjectVariant`
  - _Scriptable Object_: target SO.
  - _Scriptable Object Value_: target field.

##### Code Example
``` c#
    [BehaviorNode("867BFC14-4293-4D4E-B3F0-280AD4BAA403")]
    public struct VariantNode : INodeData
    {
        public BlobVariantReader<int> IntVariant;
        public BlobVariantReaderAndWriter<float> FloatVariant;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var intVariant = IntVariant.Read(index, ref blob, ref blackboard); // get variable value
            var floatVariant = FloatVariant.Read(index, ref blob, ref blackboard);
            FloatVariant.Write(index, ref blob, ref blackboard, floatVariant + 1);
            return NodeState.Success;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {}
    }
```

#### Multiple Trees
Adding multiple `BehaviorTreeRoot` onto a single entity gameobject will create numerous behavior trees to control this single entity.
Behavior tree sorted by `Order` of `BehaviorTreeRoot`.

<img width="400" alt="" src="https://user-images.githubusercontent.com/683655/82422698-5db32100-9ab5-11ea-8bc6-eb3c67ac7676.png">

### Debug
<img width="600" alt="debug" src="https://user-images.githubusercontent.com/683655/72407368-517f2600-379a-11ea-8aa9-c72754abce9f.gif" />

### Custom behavior node

#### Action
``` c#
// most important part of node, actual logic on runtime.
[Serializable] // for debug view only
[BehaviorNode("F5C2EE7E-690A-4B5C-9489-FB362C949192")] // must add this attribute to indicate a class is a `BehaviorNode`
public struct EntityMoveNode : INodeData
{
    public float3 Velocity; // node data saved in `INodeBlob`
    
    public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
        where TNodeBlob : struct, INodeBlob
        where TBlackboard : struct, IBlackboard
    { // access and modify node data
        ref var translation = ref bb.GetDataRef<Translation>(); // get blackboard data by ref (read/write)
        var deltaTime = bb.GetData<BehaviorTreeTickDeltaTime>(); // get blackboard data by value (readonly)
        translation.Value += Velocity * deltaTime.Value;
        return NodeState.Running;
    }

    public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
        where TNodeBlob : struct, INodeBlob
        where TBlackboard : struct, IBlackboard
    {}
}

// debug view (optional)
public class EntityMoveDebugView : BTDebugView<EntityMoveNode> {}
```

#### Decorator
``` c#
// runtime behavior
[Serializable] // for debug view only
[BehaviorNode("A13666BD-48E3-414A-BD13-5C696F2EA87E", BehaviorNodeType.Decorate/*decorator must explicit declared*/)]
public struct RepeatForeverNode : INodeData
{
    public NodeState BreakStates;
    
    public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
        where TNodeBlob : struct, INodeBlob
        where TBlackboard : struct, IBlackboard
    {
        // short-cut to tick first only children
        var childState = blob.TickChildrenReturnFirstOrDefault(index, blackboard);
        if (childState == 0) // 0 means no child was ticked
                             // tick a already completed `Sequence` or `Selector` will return 0
        {
            blob.ResetChildren(index, blackboard);
            childState = blob.TickChildrenReturnFirstOrDefault(index, blackboard);
        }
        if (BreakStates.HasFlag(childState)) return childState;
        
        return NodeState.Running;
    }

    public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
        where TNodeBlob : struct, INodeBlob
        where TBlackboard : struct, IBlackboard
    {}
}

// debug view (optional)
public class BTDebugRepeatForever : BTDebugView<RepeatForeverNode> {}
```

#### Composite
``` c#
// runtime behavior
[StructLayout(LayoutKind.Explicit)] // sizeof(SelectorNode) == 0
[BehaviorNode("BD4C1D8F-BA8E-4D74-9039-7D1E6010B058", BehaviorNodeType.Composite/*composite must explicit declared*/)]
public struct SelectorNode : INodeData
{
    public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
        where TNodeBlob : struct, INodeBlob
        where TBlackboard : struct, IBlackboard
    {
        // tick children and break if child state is running or success.
        return blob.TickChildrenReturnLastOrDefault(index, blackboard, breakCheck: state => state.IsRunningOrSuccess());
    }

    public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
        where TNodeBlob : struct, INodeBlob
        where TBlackboard : struct, IBlackboard
    {}
}

// avoid debugging view since there's nothing that needs to debug for `Selector`
```

#### `EntityQuery`
The behavior tree needs some extra information for generating `EntityQuery`.

``` c#
public struct SomeNode : INodeData
{
    // read-only access
    BlobVariantReader<int> IntVariable;
    
    // read-write access (there's no write-only access)
    BlobVariantWriter<float> FloatVariable;
    
    // read-write access
    BlobVariantReaderAndWriter<double> FloatVariable;

    // leave method attribute to be empty and will generate right access of this method
    public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
        where TNodeBlob : struct, INodeBlob
        where TBlackboard : struct, IBlackboard
    {
        // generate `[ReadOnly(typeof(ReadOnlyComponent)]` on `Tick` method
        bb.GetData<ReadOnlyComponent>();
        
        // generate `[ReadWrite(typeof(ReadWriteComponent)]` on `Tick` method
        bb.GetDataRef<ReadWriteComponent>();
        
        return NodeState.Success;
    }

    // or manually declare right access types for this method
    [EntitiesBT.Core.ReadWrite(typeof(ReadWriteComponentData))]
    [EntitiesBT.Core.ReadOnly(typeof(ReadOnlyComponentData))]
    public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
        where TNodeBlob : struct, INodeBlob
        where TBlackboard : struct, IBlackboard
    {
        // generate `[ReadOnly(typeof(ReadOnlyComponent)]` on `Reset` method
        bb.GetData<ReadOnlyComponent>();
        
        // generate `[ReadWrite(typeof(ReadWriteComponent)]` on `Reset` method
        bb.GetDataRef<ReadWriteComponent>();
        
        // ...
    }
}
```

make sure to mark the outside method call with the proper access attributes to generate the appropriate access type on `Tick` or `Reset` method of the node

```c#

public static class Extension
{
    [ReadOnly(typeof(FooComponent)), ReadWrite(typeof(BarComponent))]
    public static void Call<[ReadWrite] T, [ReadOnly] U>([ReadOnly] Type type) { /* ... */ }
}

public struct SomeNode : INodeData
{
    // leave method attribute to be empty to generate automatically
    public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
        where TNodeBlob : struct, INodeBlob
        where TBlackboard : struct, IBlackboard
    {
        // the following call will generate access attributes on `Tick` like below:
        // [ReadOnly(typeof(FooComponent))]
        // [ReadWrite(typeof(BarComponent))]
        // [ReadWrite(typeof(int))]
        // [ReadOnly(typeof(float))]
        // [ReadOnly(typeof(long))]
        Extension.Call<int, float>(typeof(long));
        return NodeState.Success;
    }
}
```

#### Advanced: customize debug view
- Behavior Node example: [PrioritySelectorNode.cs](Packages/essential/Runtime/Nodes/Composites/PrioritySelectorNode.cs)
- Debug View example: [BTDebugPrioritySelector.cs](Packages/debug.component-viewer/Runtime/BTDebugPrioritySelector.cs)

#### Advanced: access other node data
`NodeBlob` stores all the behavioral tree's internal data, and it can be accessed from any node.
To access specific node data, just store its index and access it by `INodeData.GetNodeData<T>(index)`.
- Behavior Node example: [ModifyPriorityNode.cs](Packages/essential/Runtime/Nodes/Actions/ModifyPriorityNode.cs)
- Editor/Builder example: [BTModifyPriority.cs](Packages/builder.component/Runtime/Components/BTModifyPriority.cs)

#### Advanced: behavior tree component
``` c#
[BehaviorTreeComponent] // mark a component data as `BehaviorTreeComponent`
public struct BehaviorTreeTickDeltaTime : IComponentData
{
    public float Value;
}

[UpdateBefore(typeof(VirtualMachineSystem))]
public class BehaviorTreeDeltaTimeSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref BehaviorTreeTickDeltaTime deltaTime) => deltaTime.Value = Time.DeltaTime);
    }
}
```
The components of behavior will automatically add to `Entity` on the stage of converting `GameObject` to `Entity`, if `AutoAddBehaviorTreeComponents` is enabled.

<img width="600" alt="" src="https://user-images.githubusercontent.com/683655/72411453-d7549e80-37a5-11ea-925a-b3949180dd16.png" />

#### Advanced: virtual node builder
A single builder node can produce multiple behavior nodes while building.
``` C#
public class BTSequence : BTNode<SequenceNode>
{
    [Tooltip("Enable this will re-evaluate node state from the first child until running node instead of skip to the running node directly.")]
    [SerializeField] private bool _recursiveResetStatesBeforeTick;

    public override INodeDataBuilder Self => _recursiveResetStatesBeforeTick
        // add `RecursiveResetStateNode` as parent of `this` node
        ? new BTVirtualDecorator<RecursiveResetStateNode>(this)
        : base.Self
    ;
}
```

## Data Structure
``` c#
public struct NodeBlob
{
    // default data (serializable data)
    public BlobArray<int> Types; // type id of behavior node, generated from `Guid` of `BehaviorNodeAttribute`
    public BlobArray<int> EndIndices; // range of node branch must be in [nodeIndex, nodeEndIndex)
    public BlobArray<int> Offsets; // data offset of `DefaultDataBlob` of this node
    public BlobArray<byte> DefaultDataBlob; // nodes data
    
    // runtime only data (only exist on runtime)
    public BlobArray<NodeState> States; // nodes states
    // initialize from `DefaultDataBlob`
    public BlobArray<byte> RuntimeDataBlob; // same as `DefaultNodeData` but only available at runtime and will reset to `DefaultNodeData` once reset.
}
```
<img width="600" alt="data-structure" src="https://user-images.githubusercontent.com/683655/72414832-1edf2880-37ae-11ea-8ef1-146e99d30727.png" />
