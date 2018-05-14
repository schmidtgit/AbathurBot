using Abathur;
using Abathur.Core;
using Abathur.Modules;
using NydusNetwork.Logging;

namespace Launcher.Modules.Examples {
    // This demo runs best with the "AutoHarvestGather" module in the setup.json file!
    // It showcase how IReplacebleModules can be used to swap modules at runtime.
    class RandomDemo : IReplaceableModule {
        private IAbathur _abathur;
        private IIntelManager _intel;
        private ILogger _log;
        private ZergDemo _zergModule;
        private TerranDemo _terranModule;
        private ProtossDemo _protossModule;

        // Everything inheriting from IReplaceableModule is added to the IOC and can be dependency injected.
        public RandomDemo(IAbathur abathur,IIntelManager intel,TerranDemo terranModule,ProtossDemo protossModule,ZergDemo zergModule,ILogger log) {
            _terranModule = terranModule;
            _protossModule = protossModule;
            _zergModule = zergModule;
            _abathur = abathur;
            _intel = intel;
        }

        // Race might be marked as random on initialize, wait for OnStart()
        public void Initialize() { }

        // Detect the race on initialize and add the correct module
        public void OnStart() {
            switch(_intel.ParticipantRace) {
                case NydusNetwork.API.Protocol.Race.NoRace:
                    break;
                case NydusNetwork.API.Protocol.Race.Terran:
                    _abathur.AddToGameloop(_terranModule);
                    break;
                case NydusNetwork.API.Protocol.Race.Zerg:
                    _abathur.AddToGameloop(_zergModule);
                    break;
                case NydusNetwork.API.Protocol.Race.Protoss:
                    _abathur.AddToGameloop(_protossModule);
                    break;
                case NydusNetwork.API.Protocol.Race.Random:
                    _log.LogError("RandomDemo: Race could not be detected --- no was added");
                    break;
            }
        }

        public void OnStep() {}

        public void OnGameEnded() {}

        // Remember to clean after ourself...
        public void OnRestart() {
            _abathur.RemoveFromGameloop(_terranModule);
            _abathur.RemoveFromGameloop(_zergModule);
            _abathur.RemoveFromGameloop(_protossModule);
        }

        // This module inherits from IReplacebleModule too!
        public void OnAdded() => OnStart();

        public void OnRemoved() => OnRestart();
    }
}