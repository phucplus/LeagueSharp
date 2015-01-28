using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviorSharp.Components.Composites;
using BehaviorSharp.Components.Conditionals;

namespace AIM.Autoplay.Behaviors.Positioning
{
    internal class Sequences
    {
        internal Sequence LanePush = new Sequence(new Actions().PushLane, new Conditionals().ShouldPushLane, new Inverters().LowHealth);
        internal Sequence StayWithinExpRange = new Sequence(new Actions().StayWithinExpRange);
        internal Sequence CollectHealthPack = new Sequence(new Actions().CollectHealthRelic, new Conditionals().ShouldCollectHealthRelic);
        internal Sequence TryToKill = new Sequence(new Actions().KillEnemy);
    }
}
