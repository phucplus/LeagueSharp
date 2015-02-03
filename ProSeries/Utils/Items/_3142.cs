using LeagueSharp;
using LeagueSharp.Common;

namespace ProSeries.Utils.Items
{
    internal class _3142 : Item
    {
        internal override int Id
        {
            get { return 3142; }
        }

        internal override string Name
        {
            get { return "Youmuu's Ghostblade"; }
        }

        public override void Use()
        {
            var target = ProSeries.Orbwalker.GetTarget();

            if (!target.IsValid<Obj_AI_Hero>())
            {
                return;
            }

            var targetHero = (Obj_AI_Hero) target;

            if (targetHero.IsValidTarget())
            {
                LeagueSharp.Common.Items.UseItem(Id, ProSeries.Player);
            }
        }
    }
}