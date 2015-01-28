using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using ProSeries.Properties;
using SharpDX;

namespace ProSeries.Utils.Drawings
{
    internal static class Crosshair
    {
        private static readonly Dictionary<string, SpellSlot> SupportedHeros = new Dictionary<string, SpellSlot>
        {
            { "Caitlyn", SpellSlot.R },
            { "Ezreal", SpellSlot.R },
            { "Graves", SpellSlot.R },
            { "Jinx", SpellSlot.R },
            { "Varus", SpellSlot.Q }
        };

        private static Vector2 DrawPosition
        {
            get
            {
                return new Vector2(
                    Drawing.WorldToScreen(KillableEnemy.Position).X - KillableEnemy.BoundingRadius / 2f,
                    Drawing.WorldToScreen(KillableEnemy.Position).Y - KillableEnemy.BoundingRadius / 0.5f);
            }
        }

        private static bool DrawSprite
        {
            get
            {
                return KillableEnemy != null && KillableEnemy.Position.IsOnScreen() &&
                       SupportedHeros[ProSeries.Player.ChampionName].IsReady() &&
                       ProSeries.Config.SubMenu("Drawings").Item("Crosshair", true).GetValue<bool>();
            }
        }

        private static Obj_AI_Hero KillableEnemy
        {
            get
            {
                return
                    ObjectManager.Get<Obj_AI_Hero>()
                        .OrderBy(hero => hero.Health)
                        .FirstOrDefault(
                            hero =>
                                hero.IsValidTarget(3000f) &&
                                hero.Health <=
                                ProSeries.Player.GetSpellDamage(hero, SupportedHeros[ProSeries.Player.ChampionName]));
            }
        }

        internal static void Load()
        {
            if (SupportedHeros.All(hero => hero.Key != ProSeries.Player.ChampionName))
            {
                return;
            }

            new Render.Sprite(Resources.Crosshair, new Vector2())
            {
                PositionUpdate = () => DrawPosition,
                Scale = new Vector2(1f, 1f),
                VisibleCondition = sender => DrawSprite
            }.Add();

            ProSeries.Config.SubMenu("Drawings").AddItem(new MenuItem("Crosshair", "Crosshair", true).SetValue(true));
        }
    }
}