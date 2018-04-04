using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abathur.Constants;
using Abathur.Core;
using Abathur.Core.Combat;
using Abathur.Model;
using Abathur.Modules;
using Abathur.Repositories;
using NydusNetwork.API.Protocol;

namespace Launcher.Modules.Examples
{
    class MistakesDemo : IModule
    {
        private IProductionManager _productionManager;
        private ISquadRepository _squadRepo;
        private Squad _JowansBoys;
        private IIntelManager _intel;
        private ICombatManager _combatManger;
        private bool _second;
        private bool _first;

        public MistakesDemo(IProductionManager productionManager, ISquadRepository squadRepo, IIntelManager intel, ICombatManager combatManager)
        {
            _productionManager = productionManager;
            _squadRepo = squadRepo;
            _intel = intel;
            _combatManger = combatManager;
        }
        public void Initialize()
        {
            
        }

        public void OnStart()
        {
            //Generic Jowan likes BattleCruisers, but he hasn't quite grasped what it takes to get one
            _productionManager.QueueUnit(BlizzardConstants.Unit.Battlecruiser);
            //Now the productionmanager is not gonna crash or deadlock on this request, 
            //but will try to make due with what Jowan is giving it. Thus it will create
            //all the requirements for a battlecruise and then the battle cruiser itself.

            //It is however not the case for supply, energy or creep. If you need more of these
            //you will have to create them yourself.
            for(int i = 0; i < 5; i++) {
                _productionManager.QueueUnit(BlizzardConstants.Unit.SupplyDepot, spacing:0);
                _productionManager.QueueUnit(BlizzardConstants.Unit.Marine);
            }

            //tech will automatically detect requirements just as units. as a bonus
            //if there is no barracks with room for a techlab/reactor when you build it
            //it will lift-off and relocate to a place where it can be build.
            _productionManager.QueueTech(BlizzardConstants.Research.Stimpack);
            //However in both cases not buildings that are in the production queue
            //will be added again as a result of detecting requirements. Thus the
            // above will not queue an other barracks, but just make research stimpacks

            _JowansBoys = _squadRepo.Create("JowanRules");
            _intel.Handler.RegisterHandler(Case.UnitAddedSelf, JowanHandlesIt);
            _first = true;
            _second = true;
        }

        public void OnStep()
        {
            //Now the Combat Manager is not as forgiving. It expects specific commands and when an 
            //invalid point/target/ability is given The units will do nothing
            if (_intel.GameLoop>9000 && _intel.GameLoop < 9300)
            {
                if (_first) {Console.WriteLine("performing invalid actions every loop"); _first = false; }
                _combatManger.AttackMove(_JowansBoys,new Point2D { X = -400,Y = 500 });//Bogous point never do this
                _combatManger.Move(_JowansBoys,new Point2D { X = -400,Y = 500 });
                _combatManger.Attack(_JowansBoys,500);//Bogous tag never do this
                var BC =_JowansBoys.Units.First(u => u.UnitType == BlizzardConstants.Unit.Battlecruiser);
                _combatManger.UseTargetlessAbility(BlizzardConstants.Ability.GeneralStimpack, BC.Tag);
            }
            //However if you give it correct input it will execute the command as given even if you
            //Like Generic Jowan dont want to attack the enemy, but your own base
            else if (_intel.GameLoop>9300)
            {
                if (_second) {Console.WriteLine("attacking every loop"); _second = false; }
                var ownBuilding =_intel.StructuresSelf().FirstOrDefault();
                if (ownBuilding!=null)
                    _combatManger.Attack(_JowansBoys,ownBuilding.Tag);
            }
            //run the demo to see Generic Jowan get his battle cruiser and destroy his base.

        }

        public void OnGameEnded()
        {
        }

        public void OnRestart()
        {
            
        }

        public void JowanHandlesIt(IUnit unit)
        {
            _JowansBoys.AddUnit(unit);
        }
    }
}
