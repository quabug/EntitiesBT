using EntitiesBT.Nodes;

namespace EntitiesBT.DebugView
{
    // TODO: automatically generate those class by Cecil?
    public class BTDebugTimer : BTDebugView<TimerNode, TimerNode.Data> {}
    public class BTDebugDelayTimer : BTDebugView<DelayTimerNode, DelayTimerNode.Data> {}
    public class BTDebugRepeatForever : BTDebugView<RepeatForeverNode, RepeatForeverNode.Data> {}
    public class BTDebugRepeatTimes : BTDebugView<RepeatTimesNode, RepeatTimesNode.Data> {}
}
