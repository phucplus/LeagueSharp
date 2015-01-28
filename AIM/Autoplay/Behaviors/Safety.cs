using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviorSharp;
using BehaviorSharp.Components.Actions;
using BehaviorSharp.Components.Composites;
using BehaviorSharp.Components.Decorators;
using LeagueSharp;
using LeagueSharp.Common;
using Geometry = LeagueSharp.Common.Geometry;

namespace AIM.Autoplay.Behaviors
{
    class Safety
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private static Util.Objects.Turrets _turrets = new Util.Objects.Turrets();
        private static Obj_AI_Turret _nearestTurret = _turrets.EnemyTurrets.Find(t => Geometry.Distance(t, Player) < 800);

        public static Sequence GetSequence()
        {
            var UsePots = new BehaviorAction(
                () =>
                {
                    var pot = Player.InventoryItems.FirstOrDefault(i => i.Id == ItemId.Health_Potion);
                    if (pot == null)
                    {
                        return BehaviorState.Success;
                    }
                    Player.Spellbook.CastSpell(pot.SpellSlot);
                    return BehaviorState.Failure;
                });

            var UnderTurret = new Sequence(
                new Inverter(Utils.NotSafeUnderEnemyTurret(_nearestTurret)), new Inverter(Utils.IsDead()),
                new Inverter(Utils.IsPlayerRecalling()), Utils.StopOrbwalker());

            var CastRecall =
                new BehaviorAction(
                    () => Player.Spellbook.CastSpell(SpellSlot.Recall) ? BehaviorState.Success : BehaviorState.Failure);

            RecallSequence = new Sequence(PrepareRecall, CastRecall);

            // add move away from enemy
            var NormalRecallLogic = new Sequence(Utils.IsLowHealth(), UsePots, Utils.IsEnemyNear(500), RecallSequence);

            return new Sequence(NormalRecallLogic, RecallSequence);
        }
    }
}
