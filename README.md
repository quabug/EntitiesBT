[![openupm](https://img.shields.io/npm/v/entities-bt?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/entities-bt/)
# EntitiesBT
Behavior Tree framework based on and used for Unity Entities (DOTS)

## Why another Behavior Tree framework?
While developing my new game by using Unity Entities, I found that the existing BT frameworks are not support Entities out of box and also lack of compatibility with plugins like [odin](https://odininspector.com/), so I decide to write my own.

## Features
- Actions are easy to read/write data from/to entity.
- Use Component of Unity directly instead of own editor window to maximize compatibility of other plugins.
- Data-oriented design, save all nodes data into a continuous data blob ([NodeBlob.cs](Runtime/Entities/NodeBlob.cs))
- Node has no internal states.
- Separate runtime nodes and editor nodes.
- Easy to extend.
- Also compatible with Unity GameObject without any entity.
- Able to serialize behavior tree into binary file.
- Flexible thread control: force on main thread, force on job thread, controlled by behavior tree.
- Runtime debug window to show the states of nodes.

## Disadvantages
- Incompatible with burst (Won't support this in the foreseen future)
- Lack of composite nodes, like RandomSelector, PrioritySelector, etc. (TBD)
- Lack of action nodes. (Will add some actions as extension if I personally need them)

## HowTo

