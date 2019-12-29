# EntitiesBT
Behavior Tree framework based on and used for Unity Entities (DOTS)

## Why another Behavior Tree framework?
While developing my new game by using Unity Entities, I found that the existing BT frameworks are not compatible with Entities and also lack of compatibility with plugins like [odin](https://odininspector.com/), so I decide to write my own.

## Features
- Actions are easy to read/write data from/to entity.
- Use Component of Unity directly instead of own editor window to maximumize compatibility of other plugins.
- Data-oriented design, save all nodes data into a continuous data blob ([NodeBlob.cs](Runtime/Entities/NodeBlob.cs))
- Stateless nodes.
- Seperate runtime nodes and editor nodes.
- Easy to extend.
- Also compatible with Unity GameObjects without any entity.

## How to use?
TODO

### Sample: [Entity](Samples%7E/Entity)
### Sample: [Animator](Samples%7E/Animator)
### Sample: [GameObject without entity](Samples%7E/GameObjectWithoutEntity)
