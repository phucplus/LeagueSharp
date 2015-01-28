using System;
using AIM.Autoplay.Util.Helpers;
using BehaviorSharp;
using BehaviorSharp.Components.Actions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace AIM.Autoplay.Modes
{
    internal class Carry : Base
    {
        public Carry()
        {
            Game.OnGameUpdate += OnGameUpdate;
            CustomEvents.Game.OnGameLoad += OnGameLoad;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnGameUpdate -= Load.OnGameUpdate;
        }

        public override void OnGameLoad(EventArgs args)
        {
            new AutoLevel(Util.Data.AutoLevel.GetSequence());
        }

        public override void OnGameUpdate(EventArgs args)
        {
            ObjHeroes.SortHeroesListByDistance();
            ObjTurrets.UpdateTurrets();

            ImpingAintEasy();
            RefreshMinions();

            IsInDanger = ObjectManager.Player.UnderTurret(true) && InDangerUnderEnemyTurret();
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender == null || !sender.IsValid || args == null)
            {
                return;
            }

            var target = args.Target as Obj_AI_Hero;

            if (target == null || !target.IsValid)
            {
                return;
            }

            if (sender.IsMe && sender.UnderTurret(true) && target.IsEnemy)
            {
                IsInDanger = true;
            }

            if (sender is Obj_AI_Turret && target.IsMe)
            {
                IsInDanger = true;
            }

            if (sender is Obj_AI_Minion && target.IsMe)
            {
                var orbwalkingPos = new Vector2();
                orbwalkingPos.X = ObjectManager.Player.Position.X + ObjConstants.DefensiveAdditioner;
                orbwalkingPos.Y = ObjectManager.Player.Position.Y + ObjConstants.DefensiveAdditioner;
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, orbwalkingPos.To3D());
            }
        }

        public void ImpingAintEasy()
        {
            new AutoLevel(Util.Data.AutoLevel.GetSequence());
            MetaHandler.DoChecks(); //#TODO rewrite MetaHandler with BehaviorSharp

            Behaviors.MainBehavior.Root.Tick();
        }
    }
}