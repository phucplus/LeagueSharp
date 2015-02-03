using System;
using LeagueSharp;
using LeagueSharp.Common;
using ProSeries.Utils;
using ProSeries.Utils.Drawings;

namespace ProSeries
{
    internal static class ProSeries
    {
        internal static Menu Config;
        internal static Orbwalking.Orbwalker Orbwalker;
        internal static Obj_AI_Hero Player;

        internal static void Load()
        {
            try
            {
                Player = ObjectManager.Player;

                //Print the welcome message
                Game.PrintChat("Pro Series Loaded!");

                //Load the menu.
                Config = new Menu("ProSeries", "ProSeries", true);

                //Add the target selector.
                TargetSelector.AddToMenu(Config.SubMenu("Target selector"));

                //Add the orbwalking.
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

                //Add ADC items usage.
                ItemManager.Load();

                //Load the crosshair
                Crosshair.Load();

                //Check if the champion is supported
                try
                {
                    Type.GetType("ProSeries.Champions." + Player.ChampionName).GetMethod("Load").Invoke(null, null);
                }
                catch (NullReferenceException)
                {
                    Game.PrintChat(Player.ChampionName + " is not supported yet! however the orbwalking will work");
                }

                //Add the menu as main menu.
                Config.AddToMainMenu();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}