using Abathur;
using Abathur.Constants;
using Abathur.Modules;

namespace Launcher.Modules.Examples {
    // This demo runs best with the "AutoHarvestGather" module in the setup.json file!
    // It showcase how IReplacebleModules can be used to swap modules at runtime.
    class RandomDemo : IReplaceableModule {
        private IAbathur _abathur;
        private ZergDemo _zergModule;
        private TerranDemo _terranModule;
        private ProtossDemo _protossModule;
        private bool _added;

        // Everything inheriting from IReplaceableModule is added to the IOC and can be dependency injected.
        public RandomDemo(IAbathur abathur, TerranDemo terranModule, ProtossDemo protossModule, ZergDemo zergModule) {
            _terranModule = terranModule;
            _protossModule = protossModule;
            _zergModule = zergModule;
            _abathur = abathur;
        }

        // Race might be marked as random on initialize, wait for OnStart()
        public void Initialize() {}

        // Detect the race on initialize and add the correct module
        public void OnStart() {
            switch(GameConstants.ParticipantRace) {
                case NydusNetwork.API.Protocol.Race.NoRace:
                    break;
                case NydusNetwork.API.Protocol.Race.Terran:
                    _abathur.AddToGameloop(_terranModule);
                    _added = true;
                    break;
                case NydusNetwork.API.Protocol.Race.Zerg:
                    _abathur.AddToGameloop(_zergModule);
                    _added = true;
                    break;
                case NydusNetwork.API.Protocol.Race.Protoss:
                    _abathur.AddToGameloop(_protossModule);
                    _added = true;
                    break;
                case NydusNetwork.API.Protocol.Race.Random:
                    break;
            }
        }

        public void OnStep(){}

        public void OnGameEnded(){}

        // Remember to clean after ourself...
        public void OnRestart() {
            switch(GameConstants.ParticipantRace) {
                case NydusNetwork.API.Protocol.Race.NoRace:
                    break;
                case NydusNetwork.API.Protocol.Race.Terran:
                    _abathur.RemoveFromGameloop(_terranModule);
                    _added = true;
                    break;
                case NydusNetwork.API.Protocol.Race.Zerg:
                    _abathur.RemoveFromGameloop(_zergModule);
                    _added = true;
                    break;
                case NydusNetwork.API.Protocol.Race.Protoss:
                    _abathur.RemoveFromGameloop(_protossModule);
                    _added = true;
                    break;
                case NydusNetwork.API.Protocol.Race.Random:
                    break;
            }
        }

        // This module inherits from IReplacebleModule too!
        public void OnAdded(){
            OnStart();
        }

        public void OnRemoved() => OnRestart();
    }
}
