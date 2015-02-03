using System;
using AIM.Autoplay.Modes;
using AIM.Autoplay.Util.Data;
using AIM.Autoplay.Util.Helpers;
using LeagueSharp;
using LeagueSharp.Common;
using Orbwalking = AIM.Autoplay.Util.Orbwalking;

namespace AIM.Autoplay
{
    internal class Load
    {
        private static int _loadTickCount;
        public static bool ModeLoaded = false;

        public Load()
        {
            Game.OnWndProc += OnWndProc;
            CustomEvents.Game.OnGameLoad += OnGameLoad;
            Game.OnGameUpdate += OnGameUpdate;
        }

        public static void OnWndProc(EventArgs args)
        {
            //Draw AIMLoading.jpg
        }

        public void OnGameLoad(EventArgs args)
        {
            Utils.ClearConsole();

            _loadTickCount = Environment.TickCount;

            Base.Menu = new Menu("AIM", "AIM", true);

            //AIM Settings
            Base.Menu.AddItem(new MenuItem("Enabled", "Enabled").SetValue(new KeyBind(32, KeyBindType.Toggle)));
            Base.Menu.AddItem(new MenuItem("LowHealth", "Self Low Health %").SetValue(new Slider(20, 10, 50)));
            
            //Humanizer
            var move = Base.Menu.AddSubMenu(new Menu("Humanizer", "humanizer"));
            move.AddItem(new MenuItem("MovementEnabled", "Enabled").SetValue(true));
            move.AddItem(new MenuItem("MovementDelay", "Movement Delay")).SetValue(new Slider(400, 0, 1000));

            Base.Menu.AddToMainMenu();

            FileHandler.DoChecks();

            Game.PrintChat("AIM {0} Loaded!", Program.Version);
            Game.PrintChat("Don't panic, the bot will start at 60 seconds in the game.");
        }

        public static void OnGameUpdate(EventArgs args)
        {
            if (Utility.Map.GetMap().Type == Utility.Map.MapType.HowlingAbyss)
            {
                UsePorosnax();

                try
                {
                    if (!ModeLoaded &&
                        (Environment.TickCount - _loadTickCount > 60000 || ObjectManager.Player.Level > 3))
                    {
                        new Carry();
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                //Console.WriteLine("Map not yet supported, use AutoSharpporting ;)");
                new Carry();
            }
        }

        public static bool UsePorosnax()
        {
            var trinket = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Trinket);
            return trinket != null && trinket.IsReady() && ObjectManager.Player.Spellbook.CastSpell(SpellSlot.Trinket);
        }
    }
}