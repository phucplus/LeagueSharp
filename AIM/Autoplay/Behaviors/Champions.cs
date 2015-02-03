#region

using System;
using System.Collections.Generic;
using System.Linq;
using BehaviorSharp;
using BehaviorSharp.Components.Actions;
using BehaviorSharp.Components.Composites;
using BehaviorSharp.Components.Conditionals;
using LeagueSharp;
using LeagueSharp.Common;

#endregion

namespace AIM.Autoplay.Behaviors
{
    internal static class Champions
    {
        public enum Data
        {
            SpellLogic
        }

        private static readonly Dictionary<string, ChampionData> SupportedChampions =
            new Dictionary<string, ChampionData>();

        private static readonly Dictionary<SpellSlot, Spell> SpellDictionary = new Dictionary<SpellSlot, Spell>();
        private static Spell SelectedSpell;
        private static Obj_AI_Base SpellTarget;
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private static List<ItemId> _itemList = new List<ItemId>();

        static Champions()
        {
            Soraka();
        }

        #region Soraka

        public static void Soraka()
        {
            try
            {
                var sequence = new[] { 1, 2, 3, 2, 2, 4, 2, 1, 2, 3, 4, 3, 3, 1, 1, 4, 1, 3 };

                var items = new List<ItemId>
                {
                    ItemId.Ancient_Coin,
                    ItemId.Health_Potion,
                    ItemId.Health_Potion,
                    ItemId.Health_Potion,
             //       ItemId.Warding_Totem_Trinket,
                    ItemId.Nomads_Medallion,
                    ItemId.Chalice_of_Harmony,
                    ItemId.Boots_of_Speed,
                    ItemId.Athenes_Unholy_Grail,
                    ItemId.Ionian_Boots_of_Lucidity,
                    ItemId.Rylais_Crystal_Scepter,
                    ItemId.Haunting_Guise,
                    ItemId.Liandrys_Torment
                };

                #region Menu

                var menu = new Menu("Spells", "Spells");
                //Q
                menu.AddItem(new MenuItem("QEnabled", "Use Q").SetValue(true));
                //W
                menu.AddItem(new MenuItem("WEnabled", "Use W").SetValue(true));
                menu.AddItem(new MenuItem("WMinAllyHP", "W Ally Min HP %").SetValue(new Slider(30, 0, 90)));
                //E
                menu.AddItem(new MenuItem("EEnabled", "Use E").SetValue(true));
                //R
                menu.AddItem(new MenuItem("REnabled", "Use R").SetValue(true));
                menu.AddItem(new MenuItem("RSelf", "R on Self").SetValue(true));
                menu.AddItem(new MenuItem("RMinHP", "R Ally Min HP %").SetValue(new Slider(30, 0, 50)));
                menu.AddItem(new MenuItem("RMode", "R Mode").SetValue(new StringList(new[] { "R All", "R Near" })));
                menu.AddItem(new MenuItem("RDistance", "R Near Distance").SetValue(new Slider(1500, 1000, 10000)));

                #endregion

                #region Spells

                SpellDictionary.Add(SpellSlot.Q, new Spell(SpellSlot.Q, 975));
                SpellDictionary.Add(SpellSlot.W, new Spell(SpellSlot.W, 550));
                SpellDictionary.Add(SpellSlot.E, new Spell(SpellSlot.E, 925));
                SpellDictionary.Add(SpellSlot.R, new Spell(SpellSlot.R));

                SpellDictionary[SpellSlot.Q].SetSkillshot(0.5f, 300, 1750, false, SkillshotType.SkillshotCircle);
                SpellDictionary[SpellSlot.E].SetSkillshot(0.5f, 70f, 1750, false, SkillshotType.SkillshotCircle);

                #region Q

                var qGetTarget = new BehaviorAction(
                    () =>
                    {
                        SelectedSpell = SpellDictionary[SpellSlot.Q];
                        SpellTarget = TargetSelector.GetTarget(SelectedSpell.Range, TargetSelector.DamageType.Magical);
                        return BehaviorState.Success;
                    });

                var qCheckTarget = new Conditional(() => SpellTarget.IsValidTarget(SelectedSpell.Range));

                var qCast = new BehaviorAction(
                    () =>
                    {
                        SelectedSpell.Cast(SpellTarget);
                        return BehaviorState.Success;
                    });

                var qLogic = new Sequence(SpellSlot.Q.IsReady(), qGetTarget, qCheckTarget, qCast);

                #endregion

                #region W

                var wGetTarget = new BehaviorAction(
                    () =>
                    {
                        SelectedSpell = SpellDictionary[SpellSlot.W];
                        var minHP = Program.Menu.SubMenu("Spells").Item("WMinAllyHP").GetValue<Slider>().Value;
                        SpellTarget =
                            Program.Allies.FirstOrDefault(
                                h => h.Distance(Player) < SelectedSpell.Range + 200 && h.HealthPercentage() <= minHP);
                        return BehaviorState.Success;
                    });

                var wCheckTarget = new Conditional(() => SpellTarget.IsValidTarget(SelectedSpell.Range + 200, false));

                var wCast = new BehaviorAction(
                    () =>
                    {
                        SelectedSpell.Cast(SpellTarget);
                        return BehaviorState.Success;
                    });

                var wLogic = new Sequence(SpellSlot.W.IsReady(), wGetTarget, wCheckTarget, wCast);

                #endregion

                #region E

                var eGetTarget = new BehaviorAction(
                    () =>
                    {
                        SelectedSpell = SpellDictionary[SpellSlot.E];
                        SpellTarget = TargetSelector.GetTarget(SelectedSpell.Range, TargetSelector.DamageType.Magical);
                        return BehaviorState.Success;
                    });

                var eCheckTarget = new Conditional(() => SpellTarget.IsValidTarget(SelectedSpell.Range));

                var eCast = new BehaviorAction(
                    () =>
                    {
                        SelectedSpell.Cast(SpellTarget);
                        return BehaviorState.Success;
                    });

                var eLogic = new Sequence(SpellSlot.E.IsReady(), eGetTarget, eCheckTarget, eCast);

                #endregion

                #region R

                var rGetTarget = new BehaviorAction(
                    () =>
                    {
                        SelectedSpell = SpellDictionary[SpellSlot.R];
                        var minHP = Program.Menu.SubMenu("Spells").Item("RMinHP").GetValue<Slider>().Value;
                        var maxD = Program.Menu.SubMenu("Spells").Item("RDistance").GetValue<Slider>().Value;
                        SpellTarget =
                            Program.Allies.FirstOrDefault(
                                h => h.Distance(Player) < maxD && h.HealthPercentage() <= minHP);
                        return BehaviorState.Success;
                    });

                var rCheckTarget = new Conditional(() => SpellTarget.IsValidTarget(float.MaxValue, false));

                var rCast = new BehaviorAction(
                    () =>
                    {
                        SelectedSpell.Cast(SpellTarget);
                        return BehaviorState.Success;
                    });

                var rLogic = new Sequence(SpellSlot.R.IsReady(), rGetTarget, rCheckTarget, rCast);

                #endregion

                var spellLogic = new List<Sequence> { rLogic, wLogic, eLogic, qLogic };

                #endregion

                SupportedChampions.Add("Soraka", new ChampionData(menu, spellLogic, items, sequence));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        public static ChampionData GetChampionData()
        {
            return SupportedChampions[ObjectManager.Player.ChampionName];
        }
    }

    internal class ChampionData

    {
        private readonly List<ItemId> ItemList;
        private readonly int[] Levels;
        private readonly Menu SpellMenu;
        private readonly List<Sequence> SpellSequence;

        public ChampionData(Menu spellMenu, List<Sequence> spellLogic, List<ItemId> items, int[] levels)
        {
            SpellMenu = spellMenu;
            SpellSequence = spellLogic;
            ItemList = items;
            Levels = levels;
        }

        public void SetAutoLevel()
        {
            var level = new AutoLevel(Levels);
        }

        public List<Sequence> GetSequence()
        {
            return SpellSequence;
        }

        public Menu GetMenu()
        {
            return SpellMenu;
        }

        public List<ItemId> GetItemList()
        {
            return ItemList;
        }
    }
}