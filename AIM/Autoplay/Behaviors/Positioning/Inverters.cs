using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIM.Autoplay.Util.Objects;
using BehaviorSharp.Components.Decorators;
using LeagueSharp.Common;

namespace AIM.Autoplay.Behaviors.Positioning
{
    internal class Inverters
    {
        internal Inverter LowHealth = new Inverter(new Conditionals().LowHealth);
    }
}
