using System.Linq;
using Abathur.Constants;
using Abathur.Core;
using Abathur.Core.Combat;
using Abathur.Model;
using Abathur.Modules;
using Abathur.Repositories;
using NydusNetwork.API.Protocol;
using NydusNetwork.Logging;

namespace Launcher.Modules.Examples {
    class LazyDeveloperDemo : IModule {
        private IProductionManager _productionManager;
        private ISquadRepository _squadRepo;
        private Squad _lousBoys;
        private IIntelManager _intel;
        private ICombatManager _combatManger;
        private ILogger _log;
        private bool _second;
        private bool _first;

        public LazyDeveloperDemo(IProductionManager productionManager, ISquadRepository squadRepo, IIntelManager intel, ICombatManager combatManager, ILogger log) {
            _productionManager = productionManager;
            _squadRepo = squadRepo;
            _intel = intel;
            _combatManger = combatManager;
            _log = log;
        }

        public void Initialize() {}

        // OnStart() will always be called before a game as this module does inherit from IReplableableModule
        // it can therefore not be added/removed mid-game.
        public void OnStart() {
            // Lazy Lou likes BattleCruisers, but does not want to queue everything needed to create it.
            _productionManager.QueueUnit(BlizzardConstants.Unit.Battlecruiser);
            // The production manager is not gonna crash or deadlock on this request, 
            // but will try to make due with what Lazy Lou is giving it. Thus it will create
            // all the requirements for a battlecruise and then the battlecruiser itself.

            // Supply on the other hand will NOT be automaticly added by the production manager,
            // ... unless you add the "AutoSupply" module in the setup file and let the framework handle supply too.
            // Same goes for energy fields (Pylons) and creep.
            for(int i = 0; i < 5; i++) {
                _productionManager.QueueUnit(BlizzardConstants.Unit.SupplyDepot, spacing:0);
                _productionManager.QueueUnit(BlizzardConstants.Unit.Marine);
            }

            // Tech will automatically detect requirements just as units.
            // If there is no barracks with room for a techlab/reactor when you try to build it
            // the nearest free barrack will lift-off and relocate to a place where it can be build (and log a warning)
            _productionManager.QueueTech(BlizzardConstants.Research.Stimpack);
            // The production manager detects that TechLab requires a Barrack,
            // but it is already queued and will thus not queue another.


            // You can give squads names so other modules can look them up (as their ID are generated at runtime)
            _lousBoys = _squadRepo.Create("insert_name");
            _intel.Handler.RegisterHandler(Case.UnitAddedSelf, JowanHandlesIt);
            _first = true;
            _second = true;
        }

        public void OnStep() {
            // Now the Combat Manager is not as forgiving. It expects specific commands and when an 
            // invalid point/target/ability is given units will do nothing
            if (_intel.GameLoop>9000 && _intel.GameLoop < 9300) {
                if(_first) { _log?.LogMessage("performing invalid actions every loop"); _first = false;}
                // Bogous point - never do this
                _combatManger.AttackMove(_lousBoys,new Point2D { X = -400,Y = 500 }); 
                _combatManger.Move(_lousBoys,new Point2D { X = -400,Y = 500 });
                // Bogous tag - never do this
                _combatManger.Attack(_lousBoys,500);                                  
                var BC =_lousBoys.Units.First(u => u.UnitType == BlizzardConstants.Unit.Battlecruiser);
                _combatManger.UseTargetlessAbility(BlizzardConstants.Ability.GeneralStimpack, BC.Tag);
            }

            // However if you give it correct input it will execute the command as given
            // even if you, like Lazy Lou dont want to attack the enemy, but your own base
            else if (_intel.GameLoop>9300) {
                if(_second) { _log?.LogMessage("attacking every loop"); _second = false; }
                var ownBuilding =_intel.StructuresSelf().FirstOrDefault();
                if (ownBuilding!=null)
                    _combatManger.Attack(_lousBoys,ownBuilding.Tag);
            }

            // Include the classname of this file in the setup file to see Lazy Lou get his battle cruiser (and a reprimand from the production manager)
        }

        public void OnGameEnded(){}

        public void OnRestart(){}

        // Called by Intel Manager due to handler registred in OnStart()
        public void JowanHandlesIt(IUnit unit){
            _lousBoys.AddUnit(unit);
        }
    }
}
