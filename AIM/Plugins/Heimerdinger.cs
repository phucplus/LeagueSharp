﻿using System;
using AIM.Util;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace AIM.Plugins
{
    public class Heimerdinger : PluginBase
    {
        public Vector2 pos;
        public bool Rcast;

        public Heimerdinger()
        {
            Q = new Spell(SpellSlot.Q, 525);
            W = new Spell(SpellSlot.W, 1100);
            E = new Spell(SpellSlot.E, 925);
            R = new Spell(SpellSlot.R, 100);


            W.SetSkillshot(250f, 200, 1400, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.51f, 120, 1200, false, SkillshotType.SkillshotCircle);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (R.IsReady() && !Rcast)
                {
                    R.Cast();
                    Rcast = true;
                }
                if (W.IsReady() && Target.IsValidTarget(W.Range))
                {
                    var pred = W.GetPrediction(Target);
                    W.Cast(pred.CastPosition);
                    Rcast = false;
                }
                if (E.IsReady() && Target.IsValidTarget(E.Range))
                {
                    var pred = E.GetPrediction(Target);
                    E.Cast(pred.CastPosition);
                    Rcast = false;
                }
                if (Q.IsReady() && Player.CountEnemiesInRange(1300) >= 2)
                {
                    var rnd = new Random();
                    pos.X = Player.Position.X + rnd.Next(-50, 50);
                    pos.Y = Player.Position.Y + rnd.Next(-50, 50);
                    Q.Cast(pos.To3D());
                    Rcast = false;
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