﻿﻿using System;
﻿using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using OC = Oracle.Program;

namespace Oracle.Extensions
{
    internal static class Cleansers
    {
        private static Menu _menuConfig, _mainMenu;
        private static readonly Obj_AI_Hero Me = ObjectManager.Player;

        public static void Initialize(Menu root)
        {
            Game.OnGameUpdate += Game_OnGameUpdate;

            _mainMenu = new Menu("Cleansers", "cmenu");
            _menuConfig = new Menu("Cleansers Config", "cconfig");

            foreach (var a in ObjectManager.Get<Obj_AI_Hero>().Where(a => a.Team == Me.Team))
                _menuConfig.AddItem(new MenuItem("cccon" + a.SkinName, "Use for " + a.SkinName)).SetValue(true);
            _mainMenu.AddSubMenu(_menuConfig);

            CreateMenuItem("Dervish Blade", "Dervish", 1);
            CreateMenuItem("Quicksilver Sash", "Quicksilver", 1);
            CreateMenuItem("Mercurial Scimitar", "Mercurial", 1);
            CreateMenuItem("Mikael's Crucible", "Mikaels", 1);
            _mainMenu.AddItem(new MenuItem("cleansedelay", "Cleanse delay ")).SetValue(new Slider(0));

            _mainMenu.AddItem(
                new MenuItem("cmode", "Mode: "))
                .SetValue(new StringList(new[] {"Always", "Combo"}));

            root.AddSubMenu(_mainMenu);
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            if (!OC.Origin.Item("ComboKey").GetValue<KeyBind>().Active &&
                _mainMenu.Item("cmode").GetValue<StringList>().SelectedIndex == 1)
            {
                return;
            }

            UseItem("Mikaels", 3222, 600f);
            UseItem("Quicksilver", 3140);
            UseItem("Mercurial", 3139);
            UseItem("Dervish", 3137);
        }

        private static void UseItem(string name, int itemId, float range = float.MaxValue)
        {
            if (!Items.CanUseItem(itemId) || !Items.HasItem(itemId))
                return;

            if (!_mainMenu.Item("use" + name).GetValue<bool>())
                return;

            var target = range > 5000 ? Me : OC.Friendly();
            if (_mainMenu.Item("cccon" + target.SkinName).GetValue<bool>())
            {
                if (target.Distance(Me.ServerPosition, true) <= range * range)
                {
                    var tHealthPercent = target.Health/target.MaxHealth*100;
                    var delay = _mainMenu.Item("cleansedelay").GetValue<Slider>().Value * 10;
                    foreach (var buff in GameBuff.CleanseBuffs)
                    {
                        var buffinst = target.Buffs;
                        if (buffinst.Any(aura => aura.Name.ToLower() == buff.BuffName))
                        {
                            Utility.DelayAction.Add(delay + buff.Delay, () => Items.UseItem(itemId, target));
                            OC.Logger(OC.LogType.Action,
                                "Used cleanser on " + target.SkinName + " (" + tHealthPercent + "%) for: " + buff.BuffName);
                        }
                    }

                    if (OC.Origin.Item("slow").GetValue<bool>() && target.HasBuffOfType(BuffType.Slow))
                    {
                        Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                        OC.Logger(OC.LogType.Action,
                            "Used cleanser on " + target.SkinName + " (" + tHealthPercent + "%) (slow)");
                    }

                    if (OC.Origin.Item("stun").GetValue<bool>() && target.HasBuffOfType(BuffType.Stun))
                    {
                        Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                        OC.Logger(OC.LogType.Action,
                            "Used cleanser on " + target.SkinName + " (" + tHealthPercent + "%) (stun)");
                    }

                    if (OC.Origin.Item("charm").GetValue<bool>() && target.HasBuffOfType(BuffType.Charm))
                    {
                        Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                        OC.Logger(OC.LogType.Action,
                            "Used cleanser on " + target.SkinName + " (" + tHealthPercent + "%) (charm)");
                    }

                    if (OC.Origin.Item("taunt").GetValue<bool>() && target.HasBuffOfType(BuffType.Taunt))
                    {
                        Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                        OC.Logger(OC.LogType.Action,
                            "Used cleanser on " + target.SkinName + " (" + tHealthPercent + "%) (taunt)");
                    }

                    if (OC.Origin.Item("fear").GetValue<bool>() && target.HasBuffOfType(BuffType.Fear))
                    {
                        Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                        OC.Logger(OC.LogType.Action,
                            "Used cleanser on " + target.SkinName + " (" + tHealthPercent + "%) (fear)");
                    }

                    if (OC.Origin.Item("snare").GetValue<bool>() && target.HasBuffOfType(BuffType.Snare))
                    {
                        Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                        OC.Logger(OC.LogType.Action,
                            "Used cleanser on " + target.SkinName + " (" + tHealthPercent + "%) (snare)");
                    }

                    if (OC.Origin.Item("silence").GetValue<bool>() && target.HasBuffOfType(BuffType.Silence))
                    {
                        Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                        OC.Logger(OC.LogType.Action,
                            "Used cleanser on " + target.SkinName + " (" + tHealthPercent + "%) (silence)");
                    }

                    if (OC.Origin.Item("suppression").GetValue<bool>() && target.HasBuffOfType(BuffType.Suppression))
                    {
                        Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                        OC.Logger(OC.LogType.Action,
                            "Used cleanser on " + target.SkinName + " (" + tHealthPercent + "%) (suppression)");
                    }

                    if (OC.Origin.Item("polymorph").GetValue<bool>() && target.HasBuffOfType(BuffType.Polymorph))
                    {
                        Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                        OC.Logger(OC.LogType.Action,
                            "Used cleanser on " + target.SkinName + " (" + tHealthPercent + "%) (polymorph)");
                    }

                    if (OC.Origin.Item("blind").GetValue<bool>() && target.HasBuffOfType(BuffType.Blind))
                    {
                        Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                        OC.Logger(OC.LogType.Action,
                            "Used cleanser on " + target.SkinName + " (" + tHealthPercent + "%) (blind)");
                    }

                    if (OC.Origin.Item("poison").GetValue<bool>() && target.HasBuffOfType(BuffType.Poison))
                    {
                        Utility.DelayAction.Add(delay, () => Items.UseItem(itemId, target));
                        OC.Logger(OC.LogType.Action,
                            "Used cleanser on " + target.SkinName + " (" + tHealthPercent + "%) (poison)");
                    }
                }
            }
        }

        private static void CreateMenuItem(string displayname, string name, int ccvalue)
        {
            var menuName = new Menu(name, name);
            menuName.AddItem(new MenuItem("use" + name, "Use " + displayname)).SetValue(true);
            menuName.AddItem(new MenuItem(name + "Count", "Min spells to use"));//.SetValue(new Slider(ccvalue, 1, 5));
            menuName.AddItem(new MenuItem(name + "Duration", "Buff duration to use"));//.SetValue(new Slider(2, 1, 5));
            _mainMenu.AddSubMenu(menuName);
        }
    }
}