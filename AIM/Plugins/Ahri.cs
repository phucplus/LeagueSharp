﻿using System;
using AIM.Util;
using LeagueSharp;
using LeagueSharp.Common;

namespace AIM.Plugins
{
    public class Ahri : PluginBase
    {
        public Ahri()
        {
            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E, 875);
            R = new Spell(SpellSlot.R, 850);

            Q.SetSkillshot(0.25f, 100, 1600, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 60, 1200, true, SkillshotType.SkillshotLine);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (E.CastCheck(Target, "ComboE"))
                {
                    E.CastIfHitchanceEquals(Target, HitChance.High);
                }
                if (Q.CastCheck(Target, "ComboQ") && Target.HasBuffOfType(BuffType.Charm))
                {
                    Q.Cast(Target);
                }
                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast(Target, UsePackets);
                }
                if (W.CastCheck(Target, "ComboW"))
                {
                    W.Cast();
                }
                if (R.IsReady() && ((!Q.IsReady() && !W.IsReady() && !E.IsReady()) || IsRActive()))
                {
                    R.Cast(Target);
                }
            }
            if (HarassMode)
            {
                if (E.CastCheck(Target, "ComboE"))
                {
                    E.CastIfHitchanceEquals(Target, HitChance.High);
                }
                if (Q.CastCheck(Target, "ComboQ") && Target.HasBuffOfType(BuffType.Charm))
                {
                    Q.Cast(Target);
                }
            }
        }

        private bool IsRActive()
        {
            return ObjectManager.Player.HasBuff("AhriTumble", true);
        }

        public override void OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (spell.DangerLevel < InterruptableDangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (E.CastCheck(unit, "Interrupt.E"))
            {
                E.CastIfHitchanceEquals(unit, HitChance.Medium);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R", true);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("Interrupt.E", "Use E to Interrupt Spells", true);
        }
    }
}