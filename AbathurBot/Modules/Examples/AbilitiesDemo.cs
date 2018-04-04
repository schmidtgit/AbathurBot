using System.Collections.Generic;
using System.Linq;
using Abathur.Constants;
using Abathur.Core;
using Abathur.Core.Combat;
using Abathur.Extensions;
using Abathur.Model;
using Abathur.Modules;
using Abathur.Repositories;

namespace Launcher.Modules.Examples {
    // This demo runs best with the "AutoHarvestGather" module in the setup.json file!
    // This demo contains examples of how to use different untis abilities through the CombatManager
    class AbilitiesDemo : IModule {
        private IEnumerable<IColony> _eStarts;
        private bool _done;
        private readonly IIntelManager _intelManager;
        private readonly ICombatManager _combatManager;
        private readonly IProductionManager _productionManager;
        private ISquadRepository _squadRep;
        private Squad theGang;

        public AbilitiesDemo(IIntelManager intelManager,ICombatManager combatManager,IProductionManager productionManager,ISquadRepository squadRepo) {
            _intelManager = intelManager;
            _combatManager = combatManager;
            _productionManager = productionManager;
            _squadRep = squadRepo;
        }

        public void Initialize() { }

        public void OnStart() {
            // Get all colonies that is marked as a possible enemy starting locations
            _eStarts = _intelManager.Colonies.Where(c => c.IsStartingLocation);
            
            // Register a handler to be called when a new unit friendly is added
            _intelManager.Handler.RegisterHandler(Case.UnitAddedSelf,HandleUnitMade);

            for(int i = 0; i < 7; i++)
                // Create pylons with spacing to ensure we have enough space to build.
                _productionManager.QueueUnit(BlizzardConstants.Unit.Probe,spacing: 3);

            for(int i = 0; i < 2; i++)
                // Create pylons with spacing to ensure we have enough space to build.
                _productionManager.QueueUnit(BlizzardConstants.Unit.Assimilator);

            // Produce some units with abilities
            for(int i = 0; i < 5; i++)
                // Create pylons with spacing to ensure we have enough space to build.
                _productionManager.QueueUnit(BlizzardConstants.Unit.Pylon,spacing: 3);

            for(int i = 0; i < 3; i++)
                // Create pylons with spacing to ensure we have enough space to build.
                _productionManager.QueueUnit(BlizzardConstants.Unit.Stargate,spacing: 3);


            for (int i = 0; i < 10; i++)
                _productionManager.QueueUnit(BlizzardConstants.Unit.Probe, lowPriority:true);

            // Use BlizzardConstants to get friendly names for unit type ids
            _productionManager.QueueUnit(BlizzardConstants.Unit.Assimilator);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Assimilator);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Gateway);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Gateway);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Gateway);

            for(int i = 0; i < 10; i++)
                _productionManager.QueueUnit(BlizzardConstants.Unit.Adept);

            _productionManager.QueueUnit(BlizzardConstants.Unit.Sentry);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Sentry);

            for(int i = 0; i < 10; i++)
                _productionManager.QueueUnit(BlizzardConstants.Unit.Phoenix);

            // Create a squad to control your units as one unit
            theGang = _squadRep.Create("TheGang");
        }


        public void OnStep() {
            // Wait for the queued units to finish
            if (!_done && !_intelManager.ProductionQueue.Any()) {
                foreach (var colony in _eStarts) // Then queue attack move command on each enemy starting location
                    _combatManager.AttackMove(theGang, colony.Point, true);
                _done = true; // Check to make sure we only do it once
            }

            if (_done) { // Once the attack has begun
                foreach(var gangUnit in theGang.Units) {
                    // Find the enemys closest base using straight line distance
                    var point = _eStarts.OrderBy(c => MathExtensions.EuclidianDistance(gangUnit.Point,c.Point)).First().Point;

                    // Ïf the adepts are close enough to phase walk to the middle of the base, do it.
                    if(gangUnit.UnitType == BlizzardConstants.Unit.Adept) { 
                        if(MathExtensions.EuclidianDistance(gangUnit.Point,point) < 35 && MathExtensions.EuclidianDistance(gangUnit.Point,point) > 5)
                            _combatManager.UsePointCenteredAbility(BlizzardConstants.Ability.AdeptPhaseShift, gangUnit.Tag, point);
                    } else if(gangUnit.UnitType == BlizzardConstants.Unit.Phoenix &&
                              !_intelManager.UnitsEnemyVisible.Any(u => u.BuffIds.Contains(BlizzardConstants.Buffs.GravitonBeam))) {
                        // If there is not already an enemy affected by gravition beam, then find a visible unit
                        var eUnit = _intelManager.UnitsEnemyVisible.FirstOrDefault();
                        if(eUnit != null) {
                            // and use Gravition beam on it
                            _combatManager.UseTargetedAbility(BlizzardConstants.Ability.GravitonBeam,gangUnit.Tag,eUnit.Tag);
                        }
                    } else if(gangUnit.UnitType == BlizzardConstants.Unit.Sentry) {
                        // If there are any visible enemies close
                        if(_intelManager.UnitsEnemyVisible.Any(e => MathExtensions.EuclidianDistance(e.Point,gangUnit.Point) < 15)) {
                            // Use guardian shield
                            _combatManager.UseTargetlessAbility(BlizzardConstants.Ability.GuardianShield,gangUnit.Tag);
                        }
                    }
                    if (!gangUnit.Orders.Any() && MathExtensions.EuclidianDistance(gangUnit.Point,point) > 5) // If the unit is doing nothing and is not standing on a base
                        foreach(var colony in _eStarts) // Queue an attack move command on each enemy starting location
                            _combatManager.AttackMove(theGang,colony.Point,true);
                }
            }
            
        }

        public void OnGameEnded(){}

        public void OnRestart(){}

        // Called whenever a new friendly unit is added, due to the handler registration OnStart()
        public void HandleUnitMade(IUnit unit) {
            // Whenever a unit is made, add it to the gang
            theGang.AddUnit(unit);
        }
    }
}
