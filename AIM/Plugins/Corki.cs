﻿using System;
using AIM.Util;
using LeagueSharp;
using LeagueSharp.Common;

namespace AIM.Plugins
{
    public class Corki : PluginBase
    {
        public Corki()
        {
            Author = "Shimazaki Haruka";
            Q = new Spell(SpellSlot.Q, 825f);
            E = new Spell(SpellSlot.E, 600f);
            R = new Spell(SpellSlot.R, 1300f);

            Q.SetSkillshot(0.3f, 120f, 1000f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0f, (float) (45 * Math.PI / 180), 1500, false, SkillshotType.SkillshotCone);
            R.SetSkillshot(0.2f, 40f, 2000f, true, SkillshotType.SkillshotLine);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Q.CastCheck(Target, "ComboQ"))
                {
                    Q.Cast(Target);
                }
                if (E.CastCheck(Target, "ComboE"))
                {
                    E.Cast(Target);
                }

                if (R.CastCheck(Target, "ComboR"))
                {
                    R.Cast(Target);
                }
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R", true);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("HarassQ", "Use Q", true);
            config.AddBool("HarassE", "Use E", true);
            config.AddBool("HarassR", "Use R", true);
        }
    }
}