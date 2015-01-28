#region

using System;
using System.Collections.Generic;
using System.Linq;
using BehaviorSharp;
using BehaviorSharp.Components.Composites;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

#endregion

namespace AIM.Autoplay.Behaviors
{
    internal class Program
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static float LastMove = 0;
        public static Dictionary<SpellSlot, Spell> SpellDictionary = new Dictionary<SpellSlot, Spell>();
        public static List<Obj_AI_Hero> Allies = new List<Obj_AI_Hero>();
        public static List<Obj_AI_Hero> Enemies = new List<Obj_AI_Hero>();
        public static Vector3 Spawn;
        public static Sequence HealthBehavior;
        public static List<Sequence> SpellBehavior;
        public static Sequence ShopBehavior;
        public static Behavior MainBehavior;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Menu Menu;
        public static Sequence FollowBehavior;
        public static ChampionData ChampData;
        public static bool FinishedShopping = false;
        public static float LastShop = 0;

        /*private static void Main(string[] args)
        {
            Console.Clear();
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }*/

        private static void Game_OnGameLoad(EventArgs args)
        {
            try
            {
                Allies = ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsAlly && !h.IsMe).ToList();
                Enemies = ObjectManager.Get<Obj_AI_Hero>().Where(h => h.IsEnemy).ToList();
                Spawn =
                    ObjectManager.Get<GameObject>().First(x => x is Obj_SpawnPoint && x.Team == Player.Team).Position;

                ChampData = Champions.GetChampionData();
                ChampData.SetAutoLevel();

                //temp fix
                if (Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                {
                    Player.Spellbook.LevelUpSpell(SpellSlot.Q);
                }

                /*var loadMenu = new BehaviorAction(
                    () =>
                    {
                        Menu = new Menu("AIM", "AIM", true);

                        Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
                        Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalker"));
                        Orbwalking.SetMovementDelay(350);
                        Menu.AddSubMenu(ChampData.GetMenu());

                        Menu.AddItem(new MenuItem("Enabled", "Enabled").SetValue(new KeyBind(32, KeyBindType.Toggle)));
                        Menu.AddItem(new MenuItem("LowHealth", "Self Low Health %").SetValue(new Slider(20, 10, 50)));
                        Menu.AddToMainMenu();

                        return BehaviorState.Success;
                    });

                new Behavior(loadMenu).Tick();
                */

                Menu = new Menu("AIM", "AIM", true);

                Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
                Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalker"));
                Orbwalking.SetMovementDelay(400);
                Menu.AddSubMenu(ChampData.GetMenu());

                Menu.AddItem(new MenuItem("Enabled", "Enabled").SetValue(new KeyBind(32, KeyBindType.Toggle)));
                Menu.AddItem(new MenuItem("LowHealth", "Self Low Health %").SetValue(new Slider(20, 10, 50)));
                Menu.AddToMainMenu();


                SetSequences();
                Game.OnGameUpdate += Game_OnGameUpdate;
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (!Menu.Item("Enabled").GetValue<KeyBind>().Active) { }
            MainBehavior.Tick();
        }

        private static void SetSequences()
        {
            ShopBehavior = Shop.GetSequence();
            FollowBehavior = Follow.GetSequence();
            SpellBehavior = ChampData.GetSequence();
            var AggroBehavior = new List<Sequence> { Follow.GetSequence() };
            AggroBehavior.AddRange(ChampData.GetSequence());
            HealthBehavior = Recalling.GetSequence();

            MainBehavior = new Behavior(
                new IndexSelector(
                    () =>
                    {
                        if ((Player.IsDead || Player.InFountain())) //!LeagueLib.Shop.IsFinishedShopping() && (Player.IsDead || Player.InFountain())) no idea what leaguelib is it's not even included o.O
                        {
                 //           Console.WriteLine("YESD");
                            return 0;
                        }

                        return (Utils.IsPlayerLowHealth()) ? 1 : 2;
                    }, new Sequence(), HealthBehavior, AggroBehavior.Tick()));
            
        }

        public static bool IsBadFollowTarget(Obj_AI_Hero unit)
        {
            var BadBuffs = new List<string> { "recall", "teleport" };
            return !unit.IsValidTarget(float.MaxValue, false) || unit.IsMe ||
                   unit.Distance(new Vector2(Spawn.X, Spawn.Y)) < 400;// ||
               //    unit.Buffs.Any(buff => BadBuffs.Any(buffName => buff.Name.ToLower().Contains//(buffName)));
        }
    }
}