using LeagueSharp;
using LeagueSharp.Common;

namespace ProSeries.Utils.Items
{
    internal class _3153 : Item
    {
        internal override int Id
        {
            get { return 3153; }
        }

        internal override string Name
        {
            get { return "Blade of the Ruined King"; }
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

            if (targetHero.IsValidTarget(Range) &&
                ProSeries.Player.Health + ProSeries.Player.GetItemDamage(targetHero, Damage.DamageItems.Botrk) <
                ProSeries.Player.MaxHealth)
            {
                LeagueSharp.Common.Items.UseItem(Id, targetHero);
            }
        }
    }
}