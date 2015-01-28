using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using ProSeries.Utils.Drawings;

namespace ProSeries.Champions
{
    public static class Jinx
    {
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;

        public static void Load()
        {
            //Load spells
            W = new Spell(SpellSlot.W, 1500f);
            E = new Spell(SpellSlot.E, 900f);
            R = new Spell(SpellSlot.R, 2000);

            W.SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.7f, 120f, 1750f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.6f, 140f, 1700f, false, SkillshotType.SkillshotLine);

            //Spell usage.
            ProSeries.Config.SubMenu("W").AddItem(new MenuItem("UseWCombo", "Use W on combo", true).SetValue(true));
            ProSeries.Config.SubMenu("W").AddItem(new MenuItem("UseWHarass", "Use W on harass", true).SetValue(true));

            ProSeries.Config.SubMenu("E")
                .AddItem(new MenuItem("AutoEImmobile", "Use E on immobile targets", true).SetValue(true));
            ProSeries.Config.SubMenu("E")
                .AddItem(new MenuItem("AutoEClose", "Use E on closetargets", true).SetValue(true));


            ProSeries.Config.SubMenu("R").AddItem(new MenuItem("UseR", "Use R", true).SetValue(true));
            ProSeries.Config.SubMenu("R")
                .AddItem(new MenuItem("MaxRDist", "MaxDistance", true).SetValue(new Slider(1500, 0, 3000)));

            //Drawings
            Circles.Add("W Range", W);

            //Events
            Game.OnGameUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += OrbwalkingOnAfterAttack;
        }

        private static void OrbwalkingOnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsValid || !unit.IsMe)
            {
                return;
            }

            if (!target.IsValid<Obj_AI_Hero>())
            {
                return;
            }

            var targetAsHero = (Obj_AI_Hero) target;
            if (ProSeries.Player.GetSpellDamage(targetAsHero, SpellSlot.W) / W.Delay >
                ProSeries.Player.GetAutoAttackDamage(targetAsHero, true) * (1 / ProSeries.Player.AttackDelay))
            {
                W.Cast(targetAsHero);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if ((ProSeries.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo &&
                 ProSeries.Config.SubMenu("W").Item("UseWCombo", true).GetValue<bool>() ||
                 ProSeries.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed &&
                 ProSeries.Config.SubMenu("W").Item("UseWHarass", true).GetValue<bool>()) && W.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (target != null && ProSeries.Orbwalker.GetTarget() == null)
                {
                    W.Cast(target);
                }
            }

            if (ProSeries.Config.SubMenu("E").Item("AutoEImmobile", true).GetValue<bool>() && E.IsReady())
            {
                foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsValidTarget(E.Range)))
                {
                    E.CastIfHitchanceEquals(target, HitChance.Immobile);
                }
            }

            if (ProSeries.Config.SubMenu("E").Item("AutoEClose", true).GetValue<bool>() && E.IsReady())
            {
                foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsValidTarget(400) && h.IsMelee()))
                {
                    E.CastIfHitchanceEquals(target, HitChance.High);
                }
            }

            if (ProSeries.Config.SubMenu("R").Item("UseR", true).GetValue<bool>())
            {
                var maxDistance = ProSeries.Config.SubMenu("R").Item("MaxRDist", true).GetValue<Slider>().Value;
                foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsValidTarget(maxDistance)))
                {
                    var aaDamage = Orbwalking.InAutoAttackRange(target)
                        ? ProSeries.Player.GetAutoAttackDamage(target, true)
                        : 0;
                    if (target.Health - aaDamage <= ProSeries.Player.GetSpellDamage(target, SpellSlot.R))
                    {
                        R.Cast(target);
                    }
                }
            }
        }
    }
}