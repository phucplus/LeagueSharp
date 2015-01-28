using System;
using LeagueSharp.Common;

namespace ProSeries
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += GameOnOnGameLoad;
        }

        private static void GameOnOnGameLoad(EventArgs args)
        {
            ProSeries.Load();
        }
    }
}