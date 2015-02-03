﻿using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Program;

namespace Oracle.Extensions
{
    internal static class Offensives
    {
        private static Menu _mainMenu, _menuConfig;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;

            _mainMenu = new Menu("Offensives", "omenu");
            _menuConfig = new Menu("Offensive Config", "oconfig");

            foreach (var x in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy))
                _menuConfig.AddItem(new MenuItem("ouseOn" + x.SkinName, "Use for " + x.SkinName)).SetValue(true);
            _mainMenu.AddSubMenu(_menuConfig);

            CreateMenuItem("Muramana", "Muramana", 90, 30, true);
            CreateMenuItem("Tiamat/Hydra", "Hydra", 90, 30);
            CreateMenuItem("Deathfire Grasp", "DFG", 100, 30);
            CreateMenuItem("Hextech Gunblade", "Hextech", 90, 30);
            CreateMenuItem("Youmuu's Ghostblade", "Youmuus", 90, 30);
            CreateMenuItem("Bilgewater's Cutlass", "Cutlass", 90, 30);
            CreateMenuItem("Blade of the Ruined King", "Botrk", 70, 70);
            CreateMenuItem("Frost Queen's Claim", "Frostclaim", 100, 30);
            CreateMenuItem("Sword of Divine", "Divine", 90, 30);
            CreateMenuItem("Guardians Horn", "Guardians", 90, 30);
            CreateMenuItem("Blackfire Torch", "Torch", 100, 30);
            CreateMenuItem("Entropy", "Entropy", 90, 30);

            root.AddSubMenu(_mainMenu);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (!Me.IsValidTarget(300, false))
            {
                return;
            }
            
            if (_mainMenu.Item("useMuramana").GetValue<bool>())
            {
                if (OC.CanManamune)
                {
                    if (_mainMenu.Item("muraMode").GetValue<StringList>().SelectedIndex == 1 &&
                        !OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active)
                    {
                        return;
                    }

                    var manamune = Me.GetSpellSlot("Muramana");
                    if (manamune != SpellSlot.Unknown && !Me.HasBuff("Muramana"))
                    {
                        if (Me.Mana/Me.MaxMana*100 > _mainMenu.Item("useMuramanaMana").GetValue<Slider>().Value)
                            Me.Spellbook.CastSpell(manamune);

                         Utility.DelayAction.Add(400, () => OC.CanManamune = false);
                        
                    }
                }

                if (!OC.CanManamune && !OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active)
                {
                    var manamune = Me.GetSpellSlot("Muramana");
                    if (manamune != SpellSlot.Unknown && Me.HasBuff("Muramana"))
                    {
                        Me.Spellbook.CastSpell(manamune);
                    }
                }
            }

            if (OC.CurrentTarget.IsValidTarget())
            {
                if (OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active)
                {
                    UseItem("Entropy", 3184, 450f, true);
                    UseItem("Guardians", 2051, 450f);
                    UseItem("Entropy", 3184, 450f, true);
                    UseItem("Torch", 3188, 750f, true);
                    UseItem("Torch", 3188, 750f, true);
                    UseItem("Frostclaim", 3092, 850f, true);
                    UseItem("Youmuus", 3142, 650f);
                    UseItem("Hydra", 3077, 250f);
                    UseItem("Hydra", 3074, 250f);
                    UseItem("Hextech", 3146, 700f, true);
                    UseItem("Cutlass", 3144, 450f, true);
                    UseItem("Botrk", 3153, 450f, true);
                    UseItem("Divine", 3131, 650f);
                    UseItem("DFG", 3128, 750f, true);
                }
            }
        }

        private static void UseItem(string name, int itemId, float range, bool targeted = false)
        {
            var damage = 0f;
            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
                return;

            if (!_mainMenu.Item("use" + name).GetValue<bool>())
                return;

            if (itemId == 3128 || itemId == 3188)
                damage = OC.GetComboDamage(Me, OC.CurrentTarget);

            if (OC.CurrentTarget.Distance(Me.Position) <= range)
            {
                var eHealthPercent = (int)((OC.CurrentTarget.Health / OC.CurrentTarget.MaxHealth) * 100);
                var aHealthPercent = (int)((Me.Health / OC.CurrentTarget.MaxHealth) * 100);

                if (eHealthPercent <= _mainMenu.Item("use" + name + "Pct").GetValue<Slider>().Value &&
                    _mainMenu.Item("ouseOn" + OC.CurrentTarget.SkinName).GetValue<bool>())
                {
                    if (targeted && itemId == 3092)
                    {
                        var pi = new PredictionInput
                        {
                            Aoe = true,
                            Collision = false,
                            Delay = 0.0f,
                            From = Me.Position,
                            Radius = 250f,
                            Range = 850f,
                            Speed = 1500f,
                            Unit = OC.CurrentTarget,
                            Type = SkillshotType.SkillshotCircle
                        };

                        var po = Prediction.GetPrediction(pi);
                        if (po.Hitchance >= HitChance.Medium)
                        {
                            Items.UseItem(itemId, po.CastPosition);
                            OC.Logger(OC.LogType.Action,
                                "Used " + name + " near " + po.CastPosition.CountEnemiesInRange(300) + " enemies!");
                        }

                    }

                    else if (targeted)
                    {
                        if ((itemId == 3128 || itemId == 3188) && damage <= OC.CurrentTarget.Health / 2)
                            return;

                        Items.UseItem(itemId, OC.CurrentTarget);
                        OC.Logger(Program.LogType.Action, "Used " + name + " (Targeted Enemy HP) on " + OC.CurrentTarget.SkinName);
                    }

                    else
                    {
                        Items.UseItem(itemId);
                        OC.Logger(Program.LogType.Action, "Used " + name + " (Self Enemy HP) on " + OC.CurrentTarget.SkinName);
                    }
                }

                else if (aHealthPercent <= _mainMenu.Item("use" + name + "Me").GetValue<Slider>().Value &&
                         _mainMenu.Item("ouseOn" + OC.CurrentTarget.SkinName).GetValue<bool>())
                {
                    if (targeted)
                        Items.UseItem(itemId, OC.CurrentTarget);
                    else
                        Items.UseItem(itemId);

                    OC.Logger(Program.LogType.Action, "Used " + name + " (Low My HP) on " + OC.CurrentTarget.SkinName);
                }
            }
        }

        private static void CreateMenuItem(string displayname, string name, int evalue, int avalue, bool usemana = false)
        {
            var menuName = new Menu(name, name.ToLower());
            menuName.AddItem(new MenuItem("use" + name, "Use " + displayname)).SetValue(true);
            menuName.AddItem(new MenuItem("use" + name + "Pct", "Use on enemy HP %")).SetValue(new Slider(evalue));

            if (!usemana)
                menuName.AddItem(new MenuItem("use" + name + "Me", "Use on my HP %")).SetValue(new Slider(avalue));

            if (usemana)
                menuName.AddItem(new MenuItem("use" + name + "Mana", "Minimum mana % to use")).SetValue(new Slider(35));

            if (name == "Muramana")
                menuName.AddItem( new MenuItem("muraMode", " Muramana Mode: ").SetValue(new StringList(new[] {"Always", "Combo"}, 1)));

            _mainMenu.AddSubMenu(menuName);
        }
    }
}