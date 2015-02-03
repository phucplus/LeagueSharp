#region

using System;
using System.Collections.Generic;
using System.Linq;
using BehaviorSharp;
using BehaviorSharp.Components.Actions;
using BehaviorSharp.Components.Composites;
using BehaviorSharp.Components.Conditionals;
using BehaviorSharp.Components.Decorators;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

#endregion

namespace AIM.Autoplay.Behaviors
{
    internal class Follow

    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        public static Obj_AI_Hero FollowTarget;
        private static Vector3 FollowPosition;
        private static readonly Vector3 LastMovePosition = Vector3.Zero;

        public static Sequence GetSequence()
        {
            try
            {
                var PreWalk = new BehaviorAction(() => Player.IsDead ? BehaviorState.Failure : BehaviorState.Success);

            var GetTarget = new BehaviorAction(
                    () =>
                    {
                      //  Console.WriteLine("GetTarget");
                        //FollowTarget = Program.Allies.FirstOrDefault(x => !Program.IsBadFollowTarget(x));
                        FollowTarget = TargetPriority.GetPriorityHero();
                        return FollowTarget.IsValidTarget(float.MaxValue, false)
                            ? BehaviorState.Success
                            : BehaviorState.Failure;
                    });

                //recall logic
            var FollowRecallLogic = new Inverter(new Sequence(FollowTarget.IsPlayerRecalling(),
            new Inverter(Utils.IsEnemyNear(500)), Recalling.RecallSequence));

                // don't move to follow when you are already moving to it
                var IsOTW =
                    new Conditional(
                        () =>
                            Player.GetWaypoints().Count > 3 &&
                            Player.GetWaypoints().Last().Distance(FollowTarget.Position) < 200);

                //if follow target is recalling do somethign else

                var SetOrbwalkerMode = new BehaviorAction(
                    () =>
                    {
                 //       Console.WriteLine("SETMODE");
                        Orbwalking.OrbwalkingMode.Combo.SetOrbwalkingMode();
                        return BehaviorState.Success;
                    });

                var GetFollowPosition = new BehaviorAction(
                    () =>
                    {
                   //     Console.WriteLine("GetFollowPosition");
                        // if follow unit is headed towards (and within x range of ) enemy decrease this range
                        var pos = FollowTarget.ServerPosition.To2D();
                        if (FollowTarget.GetWaypoints().Count > 1)
                        {
                            var rng = new Random(Environment.TickCount);
                            var rnd = rng.Next(100);
                            if (rnd % 3 == 0)
                            {
                                pos = FollowTarget.GetWaypoints().Last();
                            }
                            else if (rnd % 2 == 0)
                            {
                                pos = FollowTarget.GetWaypoints()[2];
                            }
                        }
                        //var pos = FollowTarget.GetWaypoints().Count > 1 ? FollowTarget.GetWaypoints().Last() : FollowTarget.Position.To2D();
                        
                        var c = new Geometry.Circle(pos, 500);
                        var c2 = new Geometry.Circle(pos, 300);
                        c.ToPolygon().Draw(Color.Red, 2);
                        c2.ToPolygon().Draw(Color.Blue, 2);

                        var poly = Geometry.ClipPolygons(new List<Geometry.Polygon> { c.ToPolygon(), c2.ToPolygon() });
                        var ran = new Random();
                        FollowPosition = poly.GetRandomPoint(LastMovePosition, ran.Next(50, 70), ran.Next(100, 200));
                        return FollowPosition == Vector3.Zero ? BehaviorState.Failure : BehaviorState.Success;
                    });
                //new Inverter(IsOTW),
                //Utils.IsPlayerFullHealth()),

                var Follow = new BehaviorAction(
                    () =>
                    {
                        Console.WriteLine("FOLLOW");
                        FollowPosition.SetOrbwalkingPoint();
                        return BehaviorState.Success;
                    });


                var NeedRandom = new Conditional(() => Orbwalking.GetLastMoveTime() > 300);

                var RandomWalk = new BehaviorAction(
                    () =>
                    {
                        ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Player.Position.Randomize(50, 100));
                        return BehaviorState.Success;
                    });

                var RandomSequence = new Selector(NeedRandom, RandomWalk);

                var FollowSequence = new Sequence(GetTarget, SetOrbwalkerMode, GetFollowPosition, Follow);
                return new Sequence(PreWalk, FollowRecallLogic, FollowSequence); //, RandomSequence));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //PreWalk, 
            return new Sequence();
            // IsOTW, 
            //Utils.Dead()
            // ShouldFollow, 
            //atfountain
            //, RandomLogic));
        }
    }

    internal class TargetPriority
    {
        public enum PriorityType
        {
            AP,
            AD,
            Bruiser,
            None
        }

        public static Obj_AI_Hero GetPriorityHero()
        {
            return GetPriority(PriorityType.AD) ?? GetPriority(PriorityType.AP) ?? GetPriority(PriorityType.Bruiser);
        }

        public static Obj_AI_Hero GetPriority(PriorityType type)
        {
            var champs = new List<string>();
            switch (type)
            {
                case PriorityType.AD:
                    champs = ad.ToList();
                    break;
                case PriorityType.AP:
                    champs = ap.ToList();
                    break;
                case PriorityType.Bruiser:
                    champs = bruiser.ToList();
                    break;
            }
            return
                ObjectManager.Get<Obj_AI_Hero>()
                    .FirstOrDefault(h => h.IsValid && h.IsAlly && champs.Contains(h.ChampionName));
        }

        #region Priorities

        private static readonly string[] ad =
        {
            "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "KogMaw",
            "MissFortune", "Quinn", "Sivir", "Tristana", "Twitch", "Varus", "Vayne", "Jinx", "Lucian"
        };

        private static readonly string[] ap =
        {
            "Ahri", "Akali", "Anivia", "Annie", "Brand", "Cassiopeia", "Diana",
            "FiddleSticks", "Fizz", "Gragas", "Heimerdinger", "Karthus", "Kassadin", "Katarina", "Kayle", "Kennen",
            "Leblanc", "Lissandra", "Lux", "Malzahar", "Mordekaiser", "Morgana", "Nidalee", "Orianna", "Ryze", "Sion",
            "Swain", "Syndra", "Teemo", "TwistedFate", "Veigar", "Viktor", "Vladimir", "Xerath", "Ziggs", "Zyra",
            "Velkoz"
        };

        private static readonly string[] bruiser =
        {
            "Darius", "Elise", "Evelynn", "Fiora", "Gangplank", "Gnar", "Jayce",
            "Pantheon", "Irelia", "JarvanIV", "Jax", "Khazix", "LeeSin", "Nocturne", "Olaf", "Poppy", "Renekton",
            "Rengar", "Riven", "Shyvana", "Trundle", "Tryndamere", "Udyr", "Vi", "MonkeyKing", "XinZhao", "Aatrox",
            "Rumble", "Shaco", "MasterYi"
        };

        #endregion
    }
}