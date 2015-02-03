using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using ProSeries.Utils.Drawings;

namespace ProSeries.Champions
{
    public static class Sivir
    {
        internal static Spell QCombo;
        internal static Spell QHarass;
        internal static Spell W;
        internal static Spell E;

        public static void Load()
        {
            //Load spells
            QCombo = new Spell(SpellSlot.Q, 1250);
            QCombo.SetSkillshot(0.25f, 90f, 1350f, false, SkillshotType.SkillshotLine);

            QHarass = new Spell(SpellSlot.Q, 1000);
            QHarass.SetSkillshot(0.25f, 90f, 1350f, true, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W);

            E = new Spell(SpellSlot.E);

            //Spell usage.
            ProSeries.Config.SubMenu("Q").AddItem(new MenuItem("UseQCombo", "Use Q on combo", true).SetValue(true));
            ProSeries.Config.SubMenu("Q").AddItem(new MenuItem("UseQHarass", "Use Q on harass", true).SetValue(true));
            ProSeries.Config.SubMenu("Q")
                .AddItem(new MenuItem("AutoQImmobile", "Auto Q immobile targets", true).SetValue(true));

            ProSeries.Config.SubMenu("W").AddItem(new MenuItem("UseW", "Use W against champions", true).SetValue(true));

            ProSeries.Config.SubMenu("E")
                .AddItem(new MenuItem("UseE", "Use E against targetted spells", true).SetValue(true));

            //Drawings
            Circles.Add("Q Range", QCombo);


            //Events
            Game.OnGameUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += Orbwalking_OnAfterAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
        }

        private static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsValid<Obj_AI_Hero>())
            {
                return;
            }

            if (!ProSeries.Config.SubMenu("E").Item("UseE", true).GetValue<bool>())
            {
                return;
            }

            if (!sender.IsEnemy)
            {
                return;
            }

            if (args.Target == null || !args.Target.IsValid || !args.Target.IsMe)
            {
                return;
            }

            if (args.SData.IsAutoAttack())
            {
                return;
            }

            //Delay the Cast a bit to make it look more human
            Utility.DelayAction.Add(100, () => E.Cast());
        }

        private static void Orbwalking_OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsValid || !unit.IsMe)
            {
                return;
            }

            if (!target.IsValid<Obj_AI_Hero>())
            {
                return;
            }

            if (ProSeries.Config.SubMenu("W").Item("UseW", true).GetValue<bool>())
            {
                W.Cast();
            }
        }

        internal static void Game_OnGameUpdate(EventArgs args)
        {
            if (ProSeries.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo &&
                ProSeries.Config.SubMenu("Q").Item("UseQCombo", true).GetValue<bool>())
            {
                CastQ(false);
                return;
            }

            if (ProSeries.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed &&
                ProSeries.Config.SubMenu("Q").Item("UseQHarass", true).GetValue<bool>())
            {
                CastQ(true);
            }

            if (ProSeries.Config.SubMenu("Q").Item("AutoQImmobile", true).GetValue<bool>() && QCombo.IsReady())
            {
                foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsValidTarget(QCombo.Range)))
                {
                    QCombo.CastIfHitchanceEquals(target, HitChance.Immobile);
                    QCombo.CastIfHitchanceEquals(target, HitChance.Dashing);
                }
            }
        }

        internal static void CastQ(bool harass)
        {
            var spell = harass ? QHarass : QCombo;

            if (!spell.IsReady())
            {
                return;
            }

            var target = TargetSelector.GetTarget(spell.Range, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                spell.Cast(target);
            }
        }
    }
}