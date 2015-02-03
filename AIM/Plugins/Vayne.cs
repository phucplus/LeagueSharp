﻿using System;
using AIM.Util;
using LeagueSharp;
using LeagueSharp.Common;

namespace AIM.Plugins
{
    public class Vayne : PluginBase
    {
        public Vayne()
        {
            Q = new Spell(SpellSlot.Q);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (!ComboMode)
            {
                return;
            }

            if (E.CastCheck(Target, "ComboE"))
            {
                E.Cast(Target);
            }

            if (!Orbwalking.InAutoAttackRange(Target) || Player.HealthPercentage() <= 20)
            {
                return;
            }

            if (R.IsReady())
            {
                R.Cast();
            }

            if (Q.IsReady())
            {
                Q.Cast();
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