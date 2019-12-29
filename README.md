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
![Entity Sample](https://user-images.githubusercontent.com/683655/71561395-ddadff80-2ab0-11ea-9bd9-c5027c339331.png)
### Sample: [Animator](Samples%7E/Animator)
![Animator Sample](https://user-images.githubusercontent.com/683655/71561423-37aec500-2ab1-11ea-9eb0-fcac56e332c9.png)
### Sample: [GameObject without entity](Samples%7E/GameObjectWithoutEntity)
![GameObject Sample](https://user-images.githubusercontent.com/683655/71561413-1bab2380-2ab1-11ea-84a3-b3fa64f0592f.png)
