using LeagueSharp;
using LeagueSharp.Common;

namespace ProSeries.Utils.Items
{
    internal class _3144 : Item
    {
        internal override int Id
        {
            get { return 3144; }
        }

        internal override string Name
        {
            get { return "Bilgewater Cutlass"; }
        }

        internal override float Range
        {
            get { return 450; }
        }

        public override void Use()
        {
            var target = ProSeries.Orbwalker.GetTarget();

            if (!target.IsValid<Obj_AI_Hero>())
            {
                return;
            }

            var targetHero = (Obj_AI_Hero) target;

            if (targetHero.IsValidTarget(Range))
            {
                LeagueSharp.Common.Items.UseItem(Id, targetHero);
            }
        }
    }
}