﻿using System;
using AIM.Util;
using LeagueSharp;
using LeagueSharp.Common;

namespace AIM.Plugins
{
    public class Graves : PluginBase
    {
        public Graves()
        {
            Q = new Spell(SpellSlot.Q, 900f);
            Q.SetSkillshot(0.25f, 15f * 1.5f * (float) Math.PI / 180, 2000f, false, SkillshotType.SkillshotCone);

            W = new Spell(SpellSlot.W, 1100f);
            W.SetSkillshot(0.25f, 250f, 1650f, false, SkillshotType.SkillshotCircle);

            R = new Spell(SpellSlot.R, 1100f);
            R.SetSkillshot(0.25f, 100f, 2100f, true, SkillshotType.SkillshotLine);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Q.IsReady() && Target.IsValidTarget(Q.Range))
                {
                    Q.Cast(Target, UsePackets, true);
                }

                if (W.IsReady() && Target.IsValidTarget(W.Range))
                {
                    W.Cast(Target, UsePackets, true);
                }

                if (R.IsReady() && (R.IsKillable(Target) || R.GetHitCount(HitChance.Low) > 1))
                {
                    R.Cast(Target);
                }
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R", true);
        }
    }
}