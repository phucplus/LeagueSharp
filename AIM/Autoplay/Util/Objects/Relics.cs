using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace AIM.Autoplay.Util.Objects
{
    class Relics
    {
        public static Obj_AI_Base ClosestRelic()
        {
            var hprelics = ObjectManager.Get<Obj_AI_Base>().FindAll(
                r => r.IsValid && r.Name.Contains("HealthPack")).ToList().OrderBy(r => Heroes.Me.Distance(r, true));
            return hprelics.First();
        }
    }
}
