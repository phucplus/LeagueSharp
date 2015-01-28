using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviorSharp;
using BehaviorSharp.Components.Actions;
using BehaviorSharp.Components.Composites;
using BehaviorSharp.Components.Conditionals;
using BehaviorSharp.Components.Decorators;
using ClipperLib;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace AIM.Autoplay.Behaviors
{
    public static class Utils
    {
        public static Vector3 GetRandomPoint(this List<List<IntPoint>> points, Vector3 last, int min, int max)
        {
            if (last == Vector3.Zero)
            {
                return points.First().First().ToVector3();
            }

            foreach (var v in from pointList in points
                from point in pointList
                select point.ToVector3()
                into v
                let d = v.Distance(last)
                where d >= min && d < max && !v.IsWall()
                select v)
            {
                var vector = v.Randomize(-200, 200);
                return vector.IsWall() ? v : vector;
            }

            return Vector3.Zero;
        }

        public static Vector3 ToVector3(this IntPoint point)
        {
            return new Vector2(point.X, point.Y).To3D();
        }

        public static Conditional IsReady(this SpellSlot slot)
        {
            return new Conditional(() => ObjectManager.Player.Spellbook.CanUseSpell(slot) == SpellState.Ready);
        }

        public static Conditional IsDead()
        {
            return new Conditional(() => ObjectManager.Player.IsDead);
        }

        public static void DrawLineInWorld(Vector3 start, Vector3 end, int width, System.Drawing.Color color)
        {
            var from = Drawing.WorldToScreen(start);
            var to = Drawing.WorldToScreen(end);
            Drawing.DrawLine(from[0], from[1], to[0], to[1], width, color);
            //Drawing.DrawLine(from.X, from.Y, to.X, to.Y, width, color);
        }

        public static Conditional IsPlayerFullHealth()
        {
            return new Conditional(() => (int) ObjectManager.Player.HealthPercentage() == 100);
        }

        public static Conditional IsLowHealth()
        {
            return
                new Conditional(
                    () =>
                        ObjectManager.Player.HealthPercentage() <
                        Autoplay.Modes.Base.Menu.Item("LowHealth").GetValue<Slider>().Value);
        }

        public static Conditional IsPlayerRecalling()
        {
            return new Conditional(() => ObjectManager.Player.IsRecalling());
        }

        public static Conditional IsPlayerRecalling(this Obj_AI_Hero hero)
        {
            return new Conditional(hero.IsRecalling);
        }

        public static Conditional IsValidFollowTarget(this Obj_AI_Hero hero, float distance = float.MaxValue)
        {
            return new Conditional(() => hero.IsValidTarget(distance, false));
        }

        public static Conditional IsEnemyNear(int range)
        {
            return new Conditional(() => ObjectManager.Player.CountEnemiesInRange(range) != 0);
        }

        public static Conditional IsAtFountain()
        {
            return new Conditional(() => ObjectManager.Player.InFountain());
        }

        public static Conditional NotSafeUnderEnemyTurret(Obj_AI_Turret nearestTurret)
        {
            return new Conditional(() => ObjectManager.Player.UnderTurret(true) && Util.Helpers.MetaHandler.CountNearbyAllyMinions(nearestTurret, 750) <= 2);
        }

        public static BehaviorAction Tick(this List<Sequence> list)
        {
            return new BehaviorAction(
                () =>
                {
                    try
                    {
                        foreach (var behavior in list)
                        {
                            behavior.Tick();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    return BehaviorState.Success;
                });
        }
    }
}