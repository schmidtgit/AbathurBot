using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abathur.Constants;
using Abathur.Core;
using Abathur.Modules;
using NydusNetwork.Logging;

namespace Launcher.Modules.Examples
{
    class AllProtossTest : IReplaceableModule
    {
        private IProductionManager _production;
        private ILogger _logger;
        private IIntelManager _intel;
        private bool started;

        public AllProtossTest(IProductionManager production,IIntelManager intel, ILogger logger)
        {
            _intel = intel;
            _production = production;
            _logger = logger;
        }
        public void Initialize()
        {
            
        }

        public void OnStart()
        {
            if(started) return;
            _logger.LogInfo("Queueing Probe");
            _production.QueueUnit(BlizzardConstants.Unit.Probe);
            _logger.LogInfo("Queueing pylons");
            _production.QueueUnit(BlizzardConstants.Unit.Pylon, spacing:4);
            _production.QueueUnit(BlizzardConstants.Unit.Pylon, spacing:4);
            _production.QueueUnit(BlizzardConstants.Unit.Pylon, spacing:4);
            _production.QueueUnit(BlizzardConstants.Unit.Pylon, spacing:4);
            _production.QueueUnit(BlizzardConstants.Unit.Pylon, spacing:4);
            _logger.LogInfo("Queueing Zealot");
            _production.QueueUnit(BlizzardConstants.Unit.Zealot);
            _logger.LogInfo("Queueing Stalker");
            _production.QueueUnit(BlizzardConstants.Unit.Stalker);
            _logger.LogInfo("Queueing Sentry");
            _production.QueueUnit(BlizzardConstants.Unit.Sentry);
            _logger.LogInfo("Queueing Adept");
            _production.QueueUnit(BlizzardConstants.Unit.Adept);
            _logger.LogInfo("Queueing HighTemplar");
            _production.QueueUnit(BlizzardConstants.Unit.HighTemplar);
            _logger.LogInfo("Queueing DarkTemplar");
            _production.QueueUnit(BlizzardConstants.Unit.DarkTemplar);
            _logger.LogInfo("Queueing Immortal");
            _production.QueueUnit(BlizzardConstants.Unit.Immortal);
            _logger.LogInfo("Queueing Colossus");
            _production.QueueUnit(BlizzardConstants.Unit.Colossus);
            _logger.LogInfo("Queueing Disruptor");
            _production.QueueUnit(BlizzardConstants.Unit.Disruptor);
            _logger.LogInfo("Queueing Observer");
            _production.QueueUnit(BlizzardConstants.Unit.Observer);
            _logger.LogInfo("Queueing WarpPrism");
            _production.QueueUnit(BlizzardConstants.Unit.WarpPrism);
            _logger.LogInfo("Queueing Phoenix");
            _production.QueueUnit(BlizzardConstants.Unit.Phoenix);
            _logger.LogInfo("Queueing VoidRay");
            _production.QueueUnit(BlizzardConstants.Unit.VoidRay);
            _logger.LogInfo("Queueing Oracle");
            _production.QueueUnit(BlizzardConstants.Unit.Oracle);
            _logger.LogInfo("Queueing Carrier");
            _production.QueueUnit(BlizzardConstants.Unit.Carrier);
            _logger.LogInfo("Queueing Tempest");
            _production.QueueUnit(BlizzardConstants.Unit.Tempest);
            _logger.LogInfo("Queueing Mothership");
            _production.QueueUnit(BlizzardConstants.Unit.Mothership);
            _logger.LogInfo("Queueing Nexus");
            _production.QueueUnit(BlizzardConstants.Unit.Nexus);
            _logger.LogInfo("Queueing Assimilator");
            _production.QueueUnit(BlizzardConstants.Unit.Assimilator);
            _logger.LogInfo("Queueing Gateway");
            _production.QueueUnit(BlizzardConstants.Unit.Gateway);
            _logger.LogInfo("Queueing Forge");
            _production.QueueUnit(BlizzardConstants.Unit.Forge);
            _logger.LogInfo("Queueing CyberneticsCore");
            _production.QueueUnit(BlizzardConstants.Unit.CyberneticsCore);
            _logger.LogInfo("Queueing Photon cannon");
            _production.QueueUnit(BlizzardConstants.Unit.PhotonCannon);
            _logger.LogInfo("Queueing RoboticsFacility");
            _production.QueueUnit(BlizzardConstants.Unit.RoboticsFacility);
            _logger.LogInfo("Queueing WarpGate");
            _production.QueueUnit(BlizzardConstants.Unit.WarpGate);
            _logger.LogInfo("Queueing StarGate");
            _production.QueueUnit(BlizzardConstants.Unit.Stargate);
            _logger.LogInfo("Queueing Twilligt Council");
            _production.QueueUnit(BlizzardConstants.Unit.TwilightCouncil);
            _logger.LogInfo("Queueing Robotics bay");
            _production.QueueUnit(BlizzardConstants.Unit.RoboticsBay);
            _logger.LogInfo("Queueing FleetBeacon");
            _production.QueueUnit(BlizzardConstants.Unit.FleetBeacon);
            _logger.LogInfo("Queueing TemplarArchives");
            _production.QueueUnit(BlizzardConstants.Unit.TemplarArchive);
            _logger.LogInfo("Queueing Dark Shrine");
            _production.QueueUnit(BlizzardConstants.Unit.DarkShrine);
            started = true;
        }

        public void OnStep()
        {
            if (!_intel.ProductionQueue.Any())
            {
                _logger.LogSuccess("all protoss units were succesfully made.");
            }
        }

        public void OnGameEnded()
        {
            if(_intel.ProductionQueue.Any()) {
                _logger.LogWarning("protoss units were not made before game end");
            }
        }

        public void OnRestart()
        {
            if(_intel.ProductionQueue.Any()) {
                _logger.LogWarning("protoss units were not made before game end");
            }
        }

        public void OnAdded()
        {
            OnStart();
        }

        public void OnRemoved()
        {
            if(_intel.ProductionQueue.Any()) {
                _logger.LogWarning("protoss units were not made before removal");
            }
        }
    }
}
