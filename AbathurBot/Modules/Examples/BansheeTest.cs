using Abathur.Constants;
using Abathur.Core;
using Abathur.Modules;

namespace AbathurBot.Modules.Examples {
    class BansheeTest : IModule {
        private IProductionManager _pQueue;
        public BansheeTest(IProductionManager productionManager) {
            _pQueue = productionManager;
        }
        public void Initialize()
            => _pQueue.QueueUnit(BlizzardConstants.Unit.Banshee);
        public void OnStart() { }
        public void OnStep() { }
        public void OnGameEnded() { }
        public void OnRestart() { }

    }
}