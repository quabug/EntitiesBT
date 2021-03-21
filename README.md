```
_____      _   _ _   _           ______ _____ 
|  ___|    | | (_) | (_)          | ___ \_   _|
| |__ _ __ | |_ _| |_ _  ___  ___ | |_/ / | |  
|  __| '_ \| __| | __| |/ _ \/ __|| ___ \ | |  
| |__| | | | |_| | |_| |  __/\__ \| |_/ / | |  
\____/_| |_|\__|_|\__|_|\___||___/\____/  \_/  
                                               
```
> **Table of contents**
>   * [Why another Behavior Tree framework?](#why-another-behavior-tree-framework)
>   * [Features](#features)
>   * [Disadvantages](#disadvantages)
>   * [HowTo](#howto)
>     - [Installation](#installation)
>     - [Usage](#usage)
>       - [Create behavior tree](#create-behavior-tree)
>       - [Attach behavior tree onto Entity](#attach-behavior-tree-onto-entity)
>       - [Serialization](#serialization)
>       - [Thread control](#thread-control)
>       - [Variant](#variant)
>         - [Variant Types](#variant-types)
>         - [Variant Sources](#variant-sources)
>       - [Multiple trees](#multiple-trees)
>     - [Debug](#debug)
>     - [Custom behavior node](#custom-behavior-node)
>       - [Action](#action)
>       - [Decorator](#decorator)
>       - [Composite](#composite)
>       - [Entity Query](#entityquery)
>       - [Advanced: automatically generate unity components from nodes](#advanced-automatically-generate-unity-components-from-nodes)
>       - [Advanced: customize debug view](#advanced-customize-debug-view)
>       - [Advanced: access other node data](#advanced-access-other-node-data)
>       - [Advanced: behavior tree component](#advanced-behavior-tree-component)
>       - [Advanced: virtual node builder](#advanced-virtual-node-builder)
>   * [Data Structure](#data-structure)

Behavior Tree framework based on and used for Unity Entities (DOTS)

## Why another Behavior Tree framework?
Existing BT frameworks are not support Entities out of box.

## Features
- Actions are easy to read/write data from/to entity.
- Use Component of Unity directly instead of own editor window to maximize compatibility of other plugins.
- Data-oriented design, save all nodes data into a continuous data blob ([NodeBlob.cs](Packages/essential/Runtime/Entities/NodeBlob.cs))
- Node has no internal states.
- Separate runtime nodes and editor nodes.
- Easy to extend.
- Also compatible with Unity GameObject without any entity.
- Able to serialize behavior tree into binary file.
- Flexible thread control: force on main thread, force on job thread, controlled by behavior tree.
- Runtime debug window to show the states of nodes.
- Optimized. 0 GC allocated by behavior tree itself after initialized, only 64Byte GC allocated every tick by [`CreateArchetypeChunkArrayAsync`](Packages/essential/Runtime/Entities/VirtualMachineSystem.cs#L59). 

## Disadvantages
- Incompatible with burst.
- Lack of action nodes. (Will add some actions as extension if I personally need them)
- Not easy to modify tree structure at runtime.
- Node data must be compatible with `Blob` and created by [`BlobBuilder`](https://docs.unity3d.com/Packages/com.unity.entities@0.11/api/Unity.Entities.BlobBuilder.html)

## Packages
- [essential](Packages/essential): essential part of entities behavior tree, any extension should depend on this package.
- [codegen](Packages/codegen): automatically generate [entity query accesors](#entityquery) on the methods of nodes.
- [builder.component](Packages/builder.component): build behavior tree data from unity components.
- [builder.odin](Packages/builder.odin): advanced hierarchy builder based on Odin and its serializer.
- [builder.visual](Packages/builder.visual): build and use behavior tree by graph of DOTS visual scripting (suspended).
- [debug.component-viewer](Packages/debug.component-viewer): show selected entity with behavior tree as components in inspector of unity while running.
- [variable.scriptable-object](Packages/variable.scriptable-object): extension for using scriptable object data as variable source of behavior tree node.
- [samples](Packages/samples): samples.

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
    "com.quabug.entities-bt.essential": "1.1.1",
    "com.quabug.entities-bt.codegen": "1.0.0",
    "com.quabug.entities-bt.builder.component": "1.0.0",
    "com.quabug.entities-bt.debug.component-viewer": "1.0.0",
    "com.quabug.entities-bt.variable.scriptable-object": "1.0.0"
  },
  "scopedRegistries": [
    {
      "name": "package.openupm.com",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.quabug.entities-bt"
      ]
    }
  ]
}
```

or 

[OpenUPM](https://openupm.com/docs/getting-started.html#installing-an-upm-package):
```
openupm add com.quabug.entities-bt.essential
openupm add com.quabug.entities-bt.builder.component
openupm add com.quabug.entities-bt.debug.component-viewer
openupm add com.quabug.entities-bt.variable.scriptable-object
```

### Usage
#### Create behavior tree
<img width="600" alt="create" src="https://user-images.githubusercontent.com/683655/72404172-5b4f5c00-378f-11ea-94a1-bb8aa5eb2608.gif" />

#### Attach behavior tree onto _Entity_
<img width="600" alt="attach" src="https://user-images.githubusercontent.com/683655/83865297-97905280-a758-11ea-9dd0-0a76bf601895.gif" />

#### Serialization
<img width="600" alt="save-to-file" src="https://user-images.githubusercontent.com/683655/72407209-b7b77900-3799-11ea-9de3-0703b1936f63.gif" />

#### Thread control
<img width="400" alt="thread-control" src="https://user-images.githubusercontent.com/683655/72407274-ee8d8f00-3799-11ea-9847-76ad6fdc5a37.png" />

- Force Run on Main Thread: running on main thread only, will not use job to tick behavior tree. Safe to call `UnityEngine` method.
- Force Run on Job: running on job threads only, will not use main thread to tick behavior tree. Not safe to call `UnityEngine` method.
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
  - _Component Value Name_: which value should be access from component
  - _Copy To Local Node_: Will read component data into local node and never write back into component data. (Force `ReadOnly` access)

- `NodeVariant`: fetch data from blob of another node
  - _Node Object_: another node should be access by this variable, must be in the same behavior tree.
  - _Value Field Name_: the name of data field in another node.
  - _Access Runtime Data_:
    - false: will copy data to local blob node while building, value change of _Node Object_ won't effect variable once build.
    - true: will access data field of _Node Object_ at runtime, something like reference value of _Node Object_.

- `ScriptableObjectVariant`
  - _Scriptable Object_: target SO.
  - _Scriptable Object Value_: target field.

##### Code Example
``` c#
    public class BTVariantNode : BTNode<VariantNode>
    {
        // have to generate an interface of `Int32VariantReader` to make it possible to serialize and display by Unity.
        // see "Generate specific types` below
        [SerializeReference, SerializeReferenceButton] // neccessary for editor
        public Int32VariantReader IntReader; // an `int` variant reader

        public SingleSerializedReaderAndWriterVariant FloatRW;

        protected override unsafe void Build(ref VariantNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            // save `Int32VariantReader` as `BlobVariantReader<int>` of `VariantNode`
            IntReader.Allocate(ref builder, ref data.IntVariant, this, tree);
            FloatRW.Allocate(ref builder, ref data.FloatVariant, this, tree);
        }
    }

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
##### Generate specific types of `IVariant<T>`
Generic `IVariant<T>` cannot be serialized in Unity, since `[SerializeReference]` is not allowed on a generic type.
A specific type of `IVariant<T>` must be declared before use.
- First create a _Scriptable Object_ of _VariantGeneratorSetting_
<img width="800" alt="Snipaste_2020-03-18_18-57-30" src="https://user-images.githubusercontent.com/683655/76953861-6159e880-694a-11ea-8fbc-33a83b181ebf.png">

- Fill which _Types_ you want to use as variable property.
- Fill _Filename_, _Namespace_, etc.
- Create script from this setting and save it in _Assets_
<img width="600" alt="Snipaste_2020-03-18_18-57-36" src="https://user-images.githubusercontent.com/683655/76953872-63bc4280-694a-11ea-8f03-73af3fa2fec2.png">

- And now you are free to use specific type properties, like `float2Property` etc.

#### Multiple Trees
Add multiple `BehaviorTreeRoot` onto a single entity gameobject will create multiple behavior tree to control this single entity.
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

// builder and editor part of node
public class EntityMove : BTNode<EntityMoveNode>
{
    public Vector3 Velocity;

    protected override void Build(ref EntityMoveNode data, BlobBuilder _, ITreeNode<INodeDataBuilder>[] __)
    {
        // set `NodeData` here
        data.Velocity = Velocity;
    }
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

// builder and editor
public class BTRepeat : BTNode<RepeatForeverNode>
{
    public NodeState BreakStates;
    
    public override void Build(ref RepeatForeverNode data, BlobBuilder _, ITreeNode<INodeDataBuilder>[] __)
    {
        data.BreakStates = BreakStates;
    }
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

// builder and editor
public class BTSelector : BTNode<SelectorNode> {}

// avoid debug view since there's nothing need to be debug for `Selector`
```

#### `EntityQuery`
Behavior tree need some extra information for generating `EntityQuery`.

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

make sure to mark outside method call with right access attributes to generate right access type on `Tick` or `Reset` method of node

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

#### Advanced: automatically generate unity components from nodes
<img width="692" alt="image" src="https://user-images.githubusercontent.com/683655/88620751-56218100-d0d1-11ea-9a88-e9b2dfeee252.png">

1. Create a `NodeComponentsGenerator` scriptable object.
2. Fill values of `NodeComponentGenerator`
3. Click *GenerateComponents* in the menu of scriptable object.

#### Advanced: customize debug view
- Behavior Node example: [PrioritySelectorNode.cs](Packages/essential/Runtime/Nodes/Composites/PrioritySelectorNode.cs)
- Debug View example: [BTDebugPrioritySelector.cs](Packages/debug.component-viewer/Runtime/BTDebugPrioritySelector.cs)

#### Advanced: access other node data
`NodeBlob` store all internal data of behavior tree, and it can be access from any node.
To access specific node data, just store its index and access by `INodeData.GetNodeData<T>(index)`.
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
The components of behavior will add into `Entity` automatically on the stage of convert `GameObject` to `Entity`, if `AutoAddBehaviorTreeComponents` is enabled.

<img width="600" alt="" src="https://user-images.githubusercontent.com/683655/72411453-d7549e80-37a5-11ea-925a-b3949180dd16.png" />

#### Advanced: virtual node builder
A single builder node is able to product multiple behavior nodes while building.
``` C#
public class BTSequence : BTNode<SequenceNode>
{
    [Tooltip("Enable this will re-evaluate node state from first child until running node instead of skip to running node directly.")]
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
