using EntitiesBT.Nodes;

namespace EntitiesBT.DebugView
{
    // TODO: automatically generate those class by Cecil?
    public class BTDebugTimer : BTDebugView<TimerNode> {}
    public class BTDebugDelayTimer : BTDebugView<DelayTimerNode> {}
    public class BTDebugRepeatForever : BTDebugView<RepeatForeverNode> {}
    public class BTDebugRepeatTimes : BTDebugView<RepeatTimesNode> {}
    public class BTDebugRepeatDuration : BTDebugView<RepeatDurationNode> {}
}
