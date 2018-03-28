using Abathur.Constants;
using Launcher.Settings;
using NydusNetwork.API.Protocol;
using NydusNetwork.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace Launcher {
    /// <summary>
    /// Used to serialize into data directory as a templates, if the folder is empty - or in order to 'factory reset'.
    /// This class contains a lot of information vital to the framework, but is left out of the framework as it is subject to change (patches)
    /// </summary>
    public static class Defaults {
        /// <summary>
        /// Used for setting up the modules in the Abathur framework.
        /// </summary>
        public static AbathurSetup AbathurSetup => new AbathurSetup { IsParallelized = false, Modules = new List<string>{ "EmptyModule", "RandomDemo", "AutoHarvestGather", "AutoSupply" } };
        /// <summary>
        /// Stores game related settings used by the StarCraft II client.
        /// </summary>
        public static GameSettings GameSettings => new GameSettings {
                FolderPath = @"C:\Program Files (x86)\StarCraft II",
                ConnectionAddress = IPAddress.Loopback.ToString(),
                ConnectionServerPort = 8165,
                ConnectionClientPort = 8170,
                MultiplayerSharedPort = 8175,
                Fullscreen = false,
                ClientWindowWidth = 1024,
                ClientWindowHeight = 768,
                GameMap = "Cloud Kingdom LE",
                Realtime = false,
                DisableFog = false,
                ParticipantRace = Race.Random,
                Opponents = new Collection<PlayerSetup> { new PlayerSetup { Type = PlayerType.Computer, Race = Race.Random, Difficulty = Difficulty.VeryEasy } }
            };

        /// <summary>
        /// Maps a dictionary to MultiValuePairs (a protobuf representation of a dictionary)
        /// </summary>
        /// <param name="dictionary">The dictionary to convert</param>
        /// <returns></returns>
        public static ICollection<MultiValuePair> ToMultiValuePair(this IDictionary<uint,uint[]> dictionary) {
            List<MultiValuePair> result = new List<MultiValuePair>();
            foreach(var pair in dictionary)
                result.Add(new MultiValuePair { Key = pair.Key,Values = { pair.Value } });
            return result;
        }

        /// <summary>
        /// Maps a dictionary to ValuePairs (a protobuf representation of a dictionary)
        /// </summary>
        /// <param name="dictionary">The dictionary to convert</param>
        /// <returns></returns>
        public static ICollection<ValuePair> ToValuePair(this IDictionary<uint,uint> dictionary) {
            List<ValuePair> result = new List<ValuePair>();
            foreach(var pair in dictionary)
                result.Add(new ValuePair { Key = pair.Key,Value = pair.Value });
            return result;
        }

        /// <summary>
        /// The Abathur Framework requires knowledge of which units can produce which.
        /// This can not currently (27/03/2018) be easily derived from the StarCraft II client.
        /// Update here if Blizzard add / removes or change the tech tree.
        /// A unit may have multiple valid producers (eg. SCVs can be build by CommandCenter, OrbitalCommand and Planetary Fortress)
        /// </summary>
        public static Dictionary<uint,uint[]> UnitProducers = new Dictionary<uint,uint[]> {
            // Terran - Base
            { BlizzardConstants.Unit.SCV , new []{BlizzardConstants.Unit.CommandCenter, BlizzardConstants.Unit.OrbitalCommand, BlizzardConstants.Unit.PlanetaryFortress} },
            // Terran - Barracks
            { BlizzardConstants.Unit.Marine , new []{BlizzardConstants.Unit.Barracks} },
            { BlizzardConstants.Unit.Marauder , new []{BlizzardConstants.Unit.Barracks} },
            { BlizzardConstants.Unit.Reaper , new []{BlizzardConstants.Unit.Barracks} },
            // Terran - Factory
            { BlizzardConstants.Unit.WidowMine , new []{BlizzardConstants.Unit.Factory} },
            { BlizzardConstants.Unit.Hellion , new []{BlizzardConstants.Unit.Factory} },
            { BlizzardConstants.Unit.SiegeTank , new []{BlizzardConstants.Unit.Factory} },
            { BlizzardConstants.Unit.Cyclone , new []{BlizzardConstants.Unit.Factory} },
            // Terran - Ghost Academy
            { BlizzardConstants.Unit.Ghost , new []{BlizzardConstants.Unit.Barracks} },
            // Terran - Armory
            { BlizzardConstants.Unit.HellionTank , new []{BlizzardConstants.Unit.Armory} },
            { BlizzardConstants.Unit.Thor , new []{BlizzardConstants.Unit.Armory} },
            // Terran - StarPort
            { BlizzardConstants.Unit.VikingFighter , new []{BlizzardConstants.Unit.Starport} },
            { BlizzardConstants.Unit.Medivac , new []{BlizzardConstants.Unit.Starport} },
            { BlizzardConstants.Unit.Liberator , new []{BlizzardConstants.Unit.Starport} },
            { BlizzardConstants.Unit.Raven , new []{BlizzardConstants.Unit.Starport} },
            { BlizzardConstants.Unit.Banshee , new []{BlizzardConstants.Unit.Starport} },
            { BlizzardConstants.Unit.Battlecruiser , new []{BlizzardConstants.Unit.Starport} },
            // Terran - Build (SCV)
            { BlizzardConstants.Unit.CommandCenter , new []{BlizzardConstants.Unit.SCV} },
            { BlizzardConstants.Unit.SupplyDepot , new []{BlizzardConstants.Unit.SCV} },
            { BlizzardConstants.Unit.Refinery , new []{BlizzardConstants.Unit.SCV} },
            { BlizzardConstants.Unit.Barracks , new []{BlizzardConstants.Unit.SCV} },
            { BlizzardConstants.Unit.Factory , new []{BlizzardConstants.Unit.SCV} },
            { BlizzardConstants.Unit.Starport , new []{BlizzardConstants.Unit.SCV} },
            { BlizzardConstants.Unit.EngineeringBay , new []{BlizzardConstants.Unit.SCV} },
            { BlizzardConstants.Unit.Bunker , new []{BlizzardConstants.Unit.SCV} },
            { BlizzardConstants.Unit.SensorTower , new []{BlizzardConstants.Unit.SCV} },
            { BlizzardConstants.Unit.MissileTurret , new []{BlizzardConstants.Unit.SCV} },
            { BlizzardConstants.Unit.GhostAcademy , new []{BlizzardConstants.Unit.SCV} },
            { BlizzardConstants.Unit.Armory , new []{BlizzardConstants.Unit.SCV} },
            { BlizzardConstants.Unit.FusionCore , new []{BlizzardConstants.Unit.SCV} },
            // Terran - UpgradesSelf
            { BlizzardConstants.Unit.OrbitalCommand , new []{BlizzardConstants.Unit.CommandCenter} },
            { BlizzardConstants.Unit.PlanetaryFortress , new []{BlizzardConstants.Unit.CommandCenter} },
            // Terran - Addons
            { BlizzardConstants.Unit.TechLab , new []{BlizzardConstants.Unit.Barracks, BlizzardConstants.Unit.Factory, BlizzardConstants.Unit.Starport} },
            { BlizzardConstants.Unit.BarracksTechLab , new []{BlizzardConstants.Unit.Barracks} },
            { BlizzardConstants.Unit.FactoryTechLab , new []{BlizzardConstants.Unit.Factory} },
            { BlizzardConstants.Unit.StarportTechLab , new []{BlizzardConstants.Unit.Starport} },
            { BlizzardConstants.Unit.Reactor , new []{BlizzardConstants.Unit.Barracks, BlizzardConstants.Unit.Factory, BlizzardConstants.Unit.Starport} },
            { BlizzardConstants.Unit.BarracksReactor , new []{BlizzardConstants.Unit.Barracks} },
            { BlizzardConstants.Unit.FactoryReactor , new []{BlizzardConstants.Unit.Armory} },
            { BlizzardConstants.Unit.StarportReactor , new []{BlizzardConstants.Unit.Starport} },

            // Protoss - Base
            { BlizzardConstants.Unit.Probe , new []{BlizzardConstants.Unit.Nexus} },
            // Protoss - Gateway / WarpGate
            { BlizzardConstants.Unit.Zealot , new []{BlizzardConstants.Unit.Gateway, BlizzardConstants.Unit.WarpGate} },
            { BlizzardConstants.Unit.Stalker , new []{BlizzardConstants.Unit.Gateway, BlizzardConstants.Unit.WarpGate } },
            { BlizzardConstants.Unit.Sentry , new []{BlizzardConstants.Unit.Gateway, BlizzardConstants.Unit.WarpGate } },
            { BlizzardConstants.Unit.Adept , new []{BlizzardConstants.Unit.Gateway, BlizzardConstants.Unit.WarpGate } },
            { BlizzardConstants.Unit.HighTemplar , new []{BlizzardConstants.Unit.Gateway, BlizzardConstants.Unit.WarpGate } },
            { BlizzardConstants.Unit.DarkTemplar , new []{BlizzardConstants.Unit.Gateway, BlizzardConstants.Unit.WarpGate } },
            // Protoss - Stargate
            { BlizzardConstants.Unit.Phoenix , new []{BlizzardConstants.Unit.Stargate} },
            { BlizzardConstants.Unit.Oracle , new []{BlizzardConstants.Unit.Stargate} },
            { BlizzardConstants.Unit.VoidRay , new []{BlizzardConstants.Unit.Stargate} },
            { BlizzardConstants.Unit.Tempest , new []{BlizzardConstants.Unit.Stargate} },
            { BlizzardConstants.Unit.Carrier , new []{BlizzardConstants.Unit.Stargate} },
            // Protoss - Robotics Facility
            { BlizzardConstants.Unit.Observer , new []{BlizzardConstants.Unit.RoboticsFacility} },
            { BlizzardConstants.Unit.Immortal , new []{BlizzardConstants.Unit.RoboticsFacility} },
            { BlizzardConstants.Unit.WarpPrism , new []{BlizzardConstants.Unit.RoboticsFacility} },
            { BlizzardConstants.Unit.Colossus , new []{BlizzardConstants.Unit.RoboticsFacility} },
            { BlizzardConstants.Unit.Disruptor , new []{BlizzardConstants.Unit.RoboticsFacility} },
            // Protoss - Build (Probe)
            { BlizzardConstants.Unit.Nexus , new []{BlizzardConstants.Unit.Probe} },
            { BlizzardConstants.Unit.Pylon , new []{BlizzardConstants.Unit.Probe} },
            { BlizzardConstants.Unit.Assimilator , new []{BlizzardConstants.Unit.Probe} },
            { BlizzardConstants.Unit.Gateway , new []{BlizzardConstants.Unit.Probe} },
            { BlizzardConstants.Unit.Forge , new []{BlizzardConstants.Unit.Probe} },
            { BlizzardConstants.Unit.CyberneticsCore , new []{BlizzardConstants.Unit.Probe} },
            { BlizzardConstants.Unit.PhotonCannon , new []{BlizzardConstants.Unit.Probe} },
            { BlizzardConstants.Unit.RoboticsFacility , new []{BlizzardConstants.Unit.Probe} },
            { BlizzardConstants.Unit.ShieldBattery , new []{BlizzardConstants.Unit.Probe} },
            { BlizzardConstants.Unit.Stargate , new []{BlizzardConstants.Unit.Probe} },
            { BlizzardConstants.Unit.TemplarArchive , new []{BlizzardConstants.Unit.Probe} },
            { BlizzardConstants.Unit.DarkShrine , new []{BlizzardConstants.Unit.Probe} },
            { BlizzardConstants.Unit.TwilightCouncil , new []{BlizzardConstants.Unit.Probe} },
            { BlizzardConstants.Unit.FleetBeacon , new []{BlizzardConstants.Unit.Probe} },
            { BlizzardConstants.Unit.RoboticsBay , new []{BlizzardConstants.Unit.Probe} },
            // Protoss - Fused (Archon)
            {BlizzardConstants.Unit.Archon, new []{BlizzardConstants.Unit.HighTemplar, BlizzardConstants.Unit.DarkTemplar} },

            // Zerg - Base
            { BlizzardConstants.Unit.Queen , new []{BlizzardConstants.Unit.Hatchery, BlizzardConstants.Unit.Lair, BlizzardConstants.Unit.Hive} },
            // Zerg - Units from Larva
            { BlizzardConstants.Unit.Drone , new []{BlizzardConstants.Unit.Larva} },
            { BlizzardConstants.Unit.Zergling , new []{BlizzardConstants.Unit.Larva} },
            { BlizzardConstants.Unit.Hydralisk , new []{BlizzardConstants.Unit.Larva} },
            { BlizzardConstants.Unit.Roach , new []{BlizzardConstants.Unit.Larva} },
            { BlizzardConstants.Unit.Infestor , new []{BlizzardConstants.Unit.Larva} },
            { BlizzardConstants.Unit.SwarmHost , new []{BlizzardConstants.Unit.Larva} },
            { BlizzardConstants.Unit.Ultralisk , new []{BlizzardConstants.Unit.Larva} },
            { BlizzardConstants.Unit.Overlord , new []{BlizzardConstants.Unit.Larva} },
            { BlizzardConstants.Unit.Mutalisk , new []{BlizzardConstants.Unit.Larva} },
            { BlizzardConstants.Unit.Corruptor , new []{BlizzardConstants.Unit.Larva} },
            { BlizzardConstants.Unit.Viper , new []{BlizzardConstants.Unit.Larva} },
            // Zerg - Morphed Units
            { BlizzardConstants.Unit.Overseer , new []{BlizzardConstants.Unit.Overlord} },
            { BlizzardConstants.Unit.Ravager , new []{BlizzardConstants.Unit.Roach} },
            { BlizzardConstants.Unit.Baneling , new []{BlizzardConstants.Unit.Zergling} },
            { BlizzardConstants.Unit.BroodLord , new []{BlizzardConstants.Unit.Corruptor} },
            { BlizzardConstants.Unit.Lurker , new []{BlizzardConstants.Unit.Hydralisk} },
            // Zerg - Build (Drone)
            { BlizzardConstants.Unit.Hatchery , new []{BlizzardConstants.Unit.Drone} },
            { BlizzardConstants.Unit.SpineCrawler , new []{BlizzardConstants.Unit.Drone} },
            { BlizzardConstants.Unit.SporeCrawler , new []{BlizzardConstants.Unit.Drone} },
            { BlizzardConstants.Unit.Extractor , new []{BlizzardConstants.Unit.Drone} },
            { BlizzardConstants.Unit.SpawningPool , new []{BlizzardConstants.Unit.Drone} },
            { BlizzardConstants.Unit.EvolutionChamber , new []{BlizzardConstants.Unit.Drone} },
            { BlizzardConstants.Unit.RoachWarren , new []{BlizzardConstants.Unit.Drone} },
            { BlizzardConstants.Unit.BanelingNest , new []{BlizzardConstants.Unit.Drone} },
            { BlizzardConstants.Unit.HydraliskDen , new []{BlizzardConstants.Unit.Drone} },
            { BlizzardConstants.Unit.InfestationPit , new []{BlizzardConstants.Unit.Drone} },
            { BlizzardConstants.Unit.Spire , new []{BlizzardConstants.Unit.Drone} },
            { BlizzardConstants.Unit.NydusNetwork , new []{BlizzardConstants.Unit.Drone} },
            { BlizzardConstants.Unit.UltraliskCavern , new []{BlizzardConstants.Unit.Drone} },
            // Zerg - Morphed Buildings
            { BlizzardConstants.Unit.Lair , new []{BlizzardConstants.Unit.Hatchery} },
            { BlizzardConstants.Unit.Hive , new []{BlizzardConstants.Unit.Lair} },
            { BlizzardConstants.Unit.GreaterSpire , new []{BlizzardConstants.Unit.Spire} }
        };


        /// <summary>
        /// The Abathur Framework requires knowledge of which structures are required to train the unit (besides their producer).
        /// This can partially be derived from the StarCraft II client using the tech_requirement on UnitType data.
        /// However, units like the Mutalisk (108) have the Spire (92) marked as tech_requirement, but Greater Spire (102) will do too.
        /// This might be changed in future updates to only include 'special cases'.
        /// </summary>
        public static Dictionary<uint,uint[]> UnitRequiredBuildings = new Dictionary<uint,uint[]> {
            // Terran - Supply Depot
            { BlizzardConstants.Unit.Barracks, new []{ BlizzardConstants.Unit.SupplyDepot } },
            // Terran - Barracks
            { BlizzardConstants.Unit.Ghost, new []{ BlizzardConstants.Unit.GhostAcademy} },
            { BlizzardConstants.Unit.Factory, new []{ BlizzardConstants.Unit.Barracks } },
            { BlizzardConstants.Unit.GhostAcademy, new []{ BlizzardConstants.Unit.Barracks } },
            { BlizzardConstants.Unit.Bunker, new []{ BlizzardConstants.Unit.Barracks } },
            { BlizzardConstants.Unit.OrbitalCommand, new []{ BlizzardConstants.Unit.Barracks } },
            // Terran - Factory
            { BlizzardConstants.Unit.Thor, new []{ BlizzardConstants.Unit.Armory } },
            { BlizzardConstants.Unit.Armory, new []{ BlizzardConstants.Unit.Factory } },
            { BlizzardConstants.Unit.Starport, new []{ BlizzardConstants.Unit.Factory } },
            // Terran - StarPort
            { BlizzardConstants.Unit.Battlecruiser, new []{ BlizzardConstants.Unit.FusionCore } },
            { BlizzardConstants.Unit.FusionCore, new []{ BlizzardConstants.Unit.Starport } },
            // Terran - EngineeringBay
            { BlizzardConstants.Unit.PlanetaryFortress, new []{ BlizzardConstants.Unit.EngineeringBay } },
            { BlizzardConstants.Unit.SensorTower, new []{ BlizzardConstants.Unit.EngineeringBay } },
            { BlizzardConstants.Unit.MissileTurret, new []{ BlizzardConstants.Unit.EngineeringBay } },

            // Protoss - Nexus
            { BlizzardConstants.Unit.Gateway, new []{ BlizzardConstants.Unit.Nexus} },
            { BlizzardConstants.Unit.Forge, new []{ BlizzardConstants.Unit.Nexus} },
            // Protoss - Gateway
            { BlizzardConstants.Unit.Stalker, new []{ BlizzardConstants.Unit.CyberneticsCore} },
            { BlizzardConstants.Unit.Sentry, new []{ BlizzardConstants.Unit.CyberneticsCore} },
            { BlizzardConstants.Unit.Adept, new []{ BlizzardConstants.Unit.CyberneticsCore} },
            { BlizzardConstants.Unit.HighTemplar, new []{ BlizzardConstants.Unit.TemplarArchive} },
            { BlizzardConstants.Unit.DarkTemplar, new []{ BlizzardConstants.Unit.DarkShrine} },
            // Protoss - Robotics Facility
            { BlizzardConstants.Unit.Colossus, new []{ BlizzardConstants.Unit.RoboticsBay } },
            { BlizzardConstants.Unit.Disruptor, new []{ BlizzardConstants.Unit.RoboticsBay } },
            // Protoss - Stargate
            { BlizzardConstants.Unit.Carrier, new []{ BlizzardConstants.Unit.FleetBeacon} },
            { BlizzardConstants.Unit.Mothership, new []{ BlizzardConstants.Unit.FleetBeacon} },
            { BlizzardConstants.Unit.Tempest, new []{ BlizzardConstants.Unit.FleetBeacon} },
            // Protoss - Forge
            { BlizzardConstants.Unit.PhotonCannon, new []{ BlizzardConstants.Unit.Forge} },
            // Protoss - Cybernetics Core
            { BlizzardConstants.Unit.RoboticsFacility, new []{ BlizzardConstants.Unit.CyberneticsCore} },
            { BlizzardConstants.Unit.Stargate, new []{ BlizzardConstants.Unit.CyberneticsCore} },
            { BlizzardConstants.Unit.TwilightCouncil, new []{ BlizzardConstants.Unit.CyberneticsCore} },
            { BlizzardConstants.Unit.CyberneticsCore, new []{ BlizzardConstants.Unit.Gateway} },
            // Protoss - Robotics Facility
            { BlizzardConstants.Unit.RoboticsBay, new []{ BlizzardConstants.Unit.RoboticsFacility} },
            // Protoss - Stargate
            { BlizzardConstants.Unit.FleetBeacon, new []{ BlizzardConstants.Unit.Stargate} },
            // Protoss - Twilight Council
            { BlizzardConstants.Unit.TemplarArchive, new []{ BlizzardConstants.Unit.TwilightCouncil} },
            { BlizzardConstants.Unit.DarkShrine, new []{ BlizzardConstants.Unit.TwilightCouncil} },

            // Zerg - Spawning Pool
            { BlizzardConstants.Unit.Zergling, new []{ BlizzardConstants.Unit.SpawningPool} },
            { BlizzardConstants.Unit.Queen, new []{ BlizzardConstants.Unit.SpawningPool} },
            { BlizzardConstants.Unit.Lair, new []{ BlizzardConstants.Unit.SpawningPool} },
            { BlizzardConstants.Unit.RoachWarren, new []{ BlizzardConstants.Unit.SpawningPool} },
            { BlizzardConstants.Unit.BanelingNest, new []{ BlizzardConstants.Unit.SpawningPool} },
            { BlizzardConstants.Unit.SpineCrawler, new []{ BlizzardConstants.Unit.SpawningPool} },
            { BlizzardConstants.Unit.SporeCrawler, new []{ BlizzardConstants.Unit.SpawningPool} },
            // Zerg - Roach Warren
            { BlizzardConstants.Unit.Roach, new []{ BlizzardConstants.Unit.RoachWarren} },
            { BlizzardConstants.Unit.Ravager, new []{ BlizzardConstants.Unit.RoachWarren} },
            // Zerg - Baneling Nest
            { BlizzardConstants.Unit.Baneling, new []{ BlizzardConstants.Unit.BanelingNest} },
            // Zerg - Infestation Pit
            { BlizzardConstants.Unit.Infestor, new []{ BlizzardConstants.Unit.InfestationPit} },
            { BlizzardConstants.Unit.SwarmHost, new []{ BlizzardConstants.Unit.InfestationPit} },
            { BlizzardConstants.Unit.Hive, new []{ BlizzardConstants.Unit.InfestationPit} },
            // Zerg - Hydralisk Den
            { BlizzardConstants.Unit.Hydralisk, new []{ BlizzardConstants.Unit.HydraliskDen, BlizzardConstants.Unit.Lurker} },
            { BlizzardConstants.Unit.LurkerDen, new []{ BlizzardConstants.Unit.HydraliskDen} },
            // Zerg - Lurker Den
            { BlizzardConstants.Unit.Lurker, new []{ BlizzardConstants.Unit.LurkerDen} },
            // Zerg - Spire
            { BlizzardConstants.Unit.Mutalisk, new []{ BlizzardConstants.Unit.Spire, BlizzardConstants.Unit.GreaterSpire } },
            { BlizzardConstants.Unit.Corruptor, new []{ BlizzardConstants.Unit.Spire,BlizzardConstants.Unit.GreaterSpire } },
            // Zerg - Greater Spire
            { BlizzardConstants.Unit.BroodLord, new []{ BlizzardConstants.Unit.GreaterSpire} },
            // Zerg - Ultralisk
            { BlizzardConstants.Unit.Ultralisk, new []{ BlizzardConstants.Unit.UltraliskCavern} },
            // Zerg - Hatchery
            { BlizzardConstants.Unit.SpawningPool, new []{ BlizzardConstants.Unit.Hatchery,BlizzardConstants.Unit.Lair,BlizzardConstants.Unit.Hive} },
            { BlizzardConstants.Unit.EvolutionChamber, new []{ BlizzardConstants.Unit.Hatchery,BlizzardConstants.Unit.Lair,BlizzardConstants.Unit.Hive } },
            // Zerg - Lair
            { BlizzardConstants.Unit.Overseer, new []{ BlizzardConstants.Unit.Lair,BlizzardConstants.Unit.Hive } },
            { BlizzardConstants.Unit.HydraliskDen, new []{ BlizzardConstants.Unit.Lair,BlizzardConstants.Unit.Hive } },
            { BlizzardConstants.Unit.InfestationPit, new []{ BlizzardConstants.Unit.Lair,BlizzardConstants.Unit.Hive } },
            { BlizzardConstants.Unit.Spire, new []{ BlizzardConstants.Unit.Lair,BlizzardConstants.Unit.Hive } },
            { BlizzardConstants.Unit.NydusNetwork, new []{ BlizzardConstants.Unit.Lair,BlizzardConstants.Unit.Hive } },
            // Zerg - Hive
            { BlizzardConstants.Unit.Viper, new []{ BlizzardConstants.Unit.Hive} },
            { BlizzardConstants.Unit.UltraliskCavern, new []{ BlizzardConstants.Unit.Hive} },
            { BlizzardConstants.Unit.GreaterSpire, new []{ BlizzardConstants.Unit.Hive} },
            // Zerg - NydusNetwork
            { BlizzardConstants.Unit.NydusCanal, new []{ BlizzardConstants.Unit.NydusNetwork} },
        };

        /// <summary>
        /// The Abathur Framework requires knowledge of what unit can research which upgrades.
        /// This can not currently (27/03/2018) be easily derived from the StarCraft II client.
        /// Update here if Blizzard add / removes or change any research.
        /// </summary>
        public static Dictionary<uint,uint> ResearchProducers = new Dictionary<uint,uint> {
            // Terran - Engineering Bay
            { BlizzardConstants.Research.InfantryWeapons1, BlizzardConstants.Unit.EngineeringBay},
            { BlizzardConstants.Research.InfantryWeapons2, BlizzardConstants.Unit.EngineeringBay},
            { BlizzardConstants.Research.InfantryWeapons3, BlizzardConstants.Unit.EngineeringBay},
            { BlizzardConstants.Research.InfantryArmor1, BlizzardConstants.Unit.EngineeringBay},
            { BlizzardConstants.Research.InfantryArmor2, BlizzardConstants.Unit.EngineeringBay},
            { BlizzardConstants.Research.InfantryArmor3, BlizzardConstants.Unit.EngineeringBay},
            { BlizzardConstants.Research.HiSecAutoTracking, BlizzardConstants.Unit.EngineeringBay},
            { BlizzardConstants.Research.StructureArmorUpgrade, BlizzardConstants.Unit.EngineeringBay},
            { BlizzardConstants.Research.NeosteelFrame, BlizzardConstants.Unit.EngineeringBay},
            // Terran - Armory
            { BlizzardConstants.Research.VehicleWeapons1, BlizzardConstants.Unit.Armory},
            { BlizzardConstants.Research.VehicleWeapons2, BlizzardConstants.Unit.Armory},
            { BlizzardConstants.Research.VehicleWeapons3, BlizzardConstants.Unit.Armory},
            { BlizzardConstants.Research.ShipWeapons1, BlizzardConstants.Unit.Armory},
            { BlizzardConstants.Research.ShipWeapons2, BlizzardConstants.Unit.Armory},
            { BlizzardConstants.Research.ShipWeapons3, BlizzardConstants.Unit.Armory},
            { BlizzardConstants.Research.VehiclePlating1, BlizzardConstants.Unit.Armory},
            { BlizzardConstants.Research.VehiclePlating2, BlizzardConstants.Unit.Armory},
            { BlizzardConstants.Research.VehiclePlating3, BlizzardConstants.Unit.Armory},
            { BlizzardConstants.Research.ShipPlating1, BlizzardConstants.Unit.Armory},
            { BlizzardConstants.Research.ShipPlating2, BlizzardConstants.Unit.Armory},
            { BlizzardConstants.Research.ShipPlating3, BlizzardConstants.Unit.Armory},
            // Terran - TechLab
            { BlizzardConstants.Research.ConcussiveShells, BlizzardConstants.Unit.BarracksTechLab},
            { BlizzardConstants.Research.CombatShield, BlizzardConstants.Unit.BarracksTechLab},
            { BlizzardConstants.Research.InfernalPreigniter, BlizzardConstants.Unit.FactoryTechLab},
            { BlizzardConstants.Research.Stimpack, BlizzardConstants.Unit.BarracksTechLab},
            { BlizzardConstants.Research.DrillingClaws, BlizzardConstants.Unit.FactoryTechLab},
            { BlizzardConstants.Research.TransformationServos, BlizzardConstants.Unit.FactoryTechLab},
            { BlizzardConstants.Research.HyperflightRotors, BlizzardConstants.Unit.StarportTechLab},
            { BlizzardConstants.Research.AdvancedBallistics, BlizzardConstants.Unit.StarportTechLab},
            { BlizzardConstants.Research.CloakingField, BlizzardConstants.Unit.StarportTechLab},
            { BlizzardConstants.Research.CorvidReactor, BlizzardConstants.Unit.StarportTechLab},
            { BlizzardConstants.Research.CaduceusReactor, BlizzardConstants.Unit.StarportTechLab},
            { BlizzardConstants.Research.DurableMaterials, BlizzardConstants.Unit.StarportTechLab},
            // Terran - Fusion Core
            { BlizzardConstants.Research.WeaponRefit, BlizzardConstants.Unit.FusionCore},
            // Terran - Ghost Academy
            { BlizzardConstants.Research.PersonalCloaking, BlizzardConstants.Unit.GhostAcademy},
            
            // Protoss - Forge
            { BlizzardConstants.Research.GroundWeapons1, BlizzardConstants.Unit.Forge},
            { BlizzardConstants.Research.GroundWeapons2, BlizzardConstants.Unit.Forge},
            { BlizzardConstants.Research.GroundWeapons3, BlizzardConstants.Unit.Forge},
            { BlizzardConstants.Research.GroundArmor1, BlizzardConstants.Unit.Forge},
            { BlizzardConstants.Research.GroundArmor2, BlizzardConstants.Unit.Forge},
            { BlizzardConstants.Research.GroundArmor3, BlizzardConstants.Unit.Forge},
            { BlizzardConstants.Research.Shields1, BlizzardConstants.Unit.Forge},
            { BlizzardConstants.Research.Shields2, BlizzardConstants.Unit.Forge},
            { BlizzardConstants.Research.Shields3, BlizzardConstants.Unit.Forge},
            // Protoss - Cybernetics Core
            { BlizzardConstants.Research.AirWeapons1, BlizzardConstants.Unit.CyberneticsCore},
            { BlizzardConstants.Research.AirWeapons2, BlizzardConstants.Unit.CyberneticsCore},
            { BlizzardConstants.Research.AirWeapons3, BlizzardConstants.Unit.CyberneticsCore},
            { BlizzardConstants.Research.AirArmor1, BlizzardConstants.Unit.CyberneticsCore},
            { BlizzardConstants.Research.AirArmor2, BlizzardConstants.Unit.CyberneticsCore},
            { BlizzardConstants.Research.AirArmor3, BlizzardConstants.Unit.CyberneticsCore},
            { BlizzardConstants.Research.Hallucination, BlizzardConstants.Unit.CyberneticsCore},
            { BlizzardConstants.Research.WarpGate, BlizzardConstants.Unit.CyberneticsCore},
            // Protoss - Robotics Bay
            { BlizzardConstants.Research.GraviticBooster, BlizzardConstants.Unit.RoboticsBay},
            { BlizzardConstants.Research.GraviticDrive, BlizzardConstants.Unit.RoboticsBay},
            { BlizzardConstants.Research.ExtendedThermalLance, BlizzardConstants.Unit.RoboticsBay},
            // Protoss - Twilight Council
            { BlizzardConstants.Research.Charge, BlizzardConstants.Unit.TwilightCouncil},
            { BlizzardConstants.Research.Blink, BlizzardConstants.Unit.TwilightCouncil},
            { BlizzardConstants.Research.ResonatingGlaives, BlizzardConstants.Unit.TwilightCouncil},
            // Protoss - Fleet Beacon
            { BlizzardConstants.Research.AnionPulseCrystals, BlizzardConstants.Unit.FleetBeacon},
            { BlizzardConstants.Research.GravitonCatapult, BlizzardConstants.Unit.FleetBeacon},
            // Protoss - Templar Archives
            { BlizzardConstants.Research.PsionicStorm, BlizzardConstants.Unit.TemplarArchive},
            // Protoss - Dark Shrine
            { BlizzardConstants.Research.ShadowStride, BlizzardConstants.Unit.DarkShrine},

            // Zerg - Evolution Champer
            { BlizzardConstants.Research.MeleeAttacks1, BlizzardConstants.Unit.EvolutionChamber},
            { BlizzardConstants.Research.MeleeAttacks2, BlizzardConstants.Unit.EvolutionChamber},
            { BlizzardConstants.Research.MeleeAttacks3, BlizzardConstants.Unit.EvolutionChamber},
            { BlizzardConstants.Research.MissileAttacks1, BlizzardConstants.Unit.EvolutionChamber},
            { BlizzardConstants.Research.MissileAttacks2, BlizzardConstants.Unit.EvolutionChamber},
            { BlizzardConstants.Research.MissileAttacks3, BlizzardConstants.Unit.EvolutionChamber},
            { BlizzardConstants.Research.GroundCarapace1, BlizzardConstants.Unit.EvolutionChamber},
            { BlizzardConstants.Research.GroundCarapace2, BlizzardConstants.Unit.EvolutionChamber},
            { BlizzardConstants.Research.GroundCarapace3, BlizzardConstants.Unit.EvolutionChamber},
            // Zerg - Spire
            { BlizzardConstants.Research.FlyerAttack1, BlizzardConstants.Unit.Spire},
            { BlizzardConstants.Research.FlyerAttack2, BlizzardConstants.Unit.Spire},
            { BlizzardConstants.Research.FlyerAttack3, BlizzardConstants.Unit.Spire},
            { BlizzardConstants.Research.FlyerCarapace1, BlizzardConstants.Unit.Spire},
            { BlizzardConstants.Research.FlyerCarapace2, BlizzardConstants.Unit.Spire},
            { BlizzardConstants.Research.FlyerCarapace3, BlizzardConstants.Unit.Spire},
            // Zerg - Ultralisk Carvern
            { BlizzardConstants.Research.ChitinousPlating, BlizzardConstants.Unit.UltraliskCavern},
            // Zerg - Baneling Nest
            { BlizzardConstants.Research.CentrifugalHooks, BlizzardConstants.Unit.BanelingNest},
            // Zerg - Roach Warren
            { BlizzardConstants.Research.GlialReconstitution, BlizzardConstants.Unit.RoachWarren},
            { BlizzardConstants.Research.TunnelingClaws, BlizzardConstants.Unit.RoachWarren},
            // Zerg - Hatchery
            { BlizzardConstants.Research.PneumatizedCarapace, BlizzardConstants.Unit.Hatchery},
            { BlizzardConstants.Research.Burrow, BlizzardConstants.Unit.Hatchery},
            { BlizzardConstants.Research.EvolveVentralSacks, BlizzardConstants.Unit.Hatchery},
            // Zerg - Hydralisk Den
            { BlizzardConstants.Research.MuscularAugments, BlizzardConstants.Unit.HydraliskDen},
            // Zerg - Spawning Pool
            { BlizzardConstants.Research.MetabolicBoost, BlizzardConstants.Unit.SpawningPool},
            { BlizzardConstants.Research.AdrenalGlands, BlizzardConstants.Unit.SpawningPool},
            // Zerg - Infestation Pit
            { BlizzardConstants.Research.PathogenGlands, BlizzardConstants.Unit.InfestationPit},
            { BlizzardConstants.Research.NeuralParasite, BlizzardConstants.Unit.InfestationPit},
        };

        /// <summary>
        /// The Abathur Framework requires knowledge of what research requires which structures (besides their producer).
        /// This can not currently (27/03/2018) be easily derived from the StarCraft II client.
        /// Update here if Blizzard add / removes or change any research.
        /// </summary>
        public static Dictionary<uint,uint> ResearchRequiredBuildings = new Dictionary<uint,uint> {
            // Terran - Armory
            { BlizzardConstants.Research.InfantryWeapons2, BlizzardConstants.Unit.Armory},
            { BlizzardConstants.Research.InfantryWeapons3, BlizzardConstants.Unit.Armory},
            { BlizzardConstants.Research.InfantryArmor2, BlizzardConstants.Unit.Armory},
            { BlizzardConstants.Research.InfantryArmor3, BlizzardConstants.Unit.Armory},
            { BlizzardConstants.Research.TransformationServos, BlizzardConstants.Unit.Armory},

            // Protoss - Twillight Council
            { BlizzardConstants.Research.GroundWeapons2, BlizzardConstants.Unit.TwilightCouncil},
            { BlizzardConstants.Research.GroundWeapons3, BlizzardConstants.Unit.TwilightCouncil},
            { BlizzardConstants.Research.GroundArmor2, BlizzardConstants.Unit.TwilightCouncil},
            { BlizzardConstants.Research.GroundArmor3, BlizzardConstants.Unit.TwilightCouncil},
            { BlizzardConstants.Research.Shields2, BlizzardConstants.Unit.TwilightCouncil},
            { BlizzardConstants.Research.Shields3, BlizzardConstants.Unit.TwilightCouncil},
            // Protoss - Fleet Beacon
            { BlizzardConstants.Research.AirWeapons2, BlizzardConstants.Unit.FleetBeacon},
            { BlizzardConstants.Research.AirWeapons3, BlizzardConstants.Unit.FleetBeacon},
            { BlizzardConstants.Research.AirArmor2, BlizzardConstants.Unit.FleetBeacon},
            { BlizzardConstants.Research.AirArmor3, BlizzardConstants.Unit.FleetBeacon},

            // Zerg - Lair
            { BlizzardConstants.Research.MeleeAttacks2, BlizzardConstants.Unit.Lair},
            { BlizzardConstants.Research.MissileAttacks2, BlizzardConstants.Unit.Lair},
            { BlizzardConstants.Research.FlyerAttack2, BlizzardConstants.Unit.Lair},
            { BlizzardConstants.Research.GroundCarapace2, BlizzardConstants.Unit.Lair},
            { BlizzardConstants.Research.FlyerCarapace2, BlizzardConstants.Unit.Lair},
            { BlizzardConstants.Research.GlialReconstitution, BlizzardConstants.Unit.Lair},
            { BlizzardConstants.Research.CentrifugalHooks, BlizzardConstants.Unit.Lair},
            { BlizzardConstants.Research.EvolveVentralSacks, BlizzardConstants.Unit.Lair},
            { BlizzardConstants.Research.TunnelingClaws, BlizzardConstants.Unit.Lair},
            // Zerg - Hive
            { BlizzardConstants.Research.MeleeAttacks3, BlizzardConstants.Unit.Hive},
            { BlizzardConstants.Research.MissileAttacks3, BlizzardConstants.Unit.Hive},
            { BlizzardConstants.Research.FlyerAttack3, BlizzardConstants.Unit.Hive},
            { BlizzardConstants.Research.GroundCarapace3, BlizzardConstants.Unit.Hive},
            { BlizzardConstants.Research.FlyerCarapace3, BlizzardConstants.Unit.Hive},
            { BlizzardConstants.Research.AdrenalGlands, BlizzardConstants.Unit.Hive},
        };

        /// <summary>
        /// The Abathur Framework requires knowledge of what research requires which structures (besides their producer).
        /// This can be derived because they all have a number in their name (eg. GroundWeapons1 (39), GroundWeapons2 (40), GroundWeapons3 (41))
        /// However, we prefer to have it stored explicit to prevent false assumptions and stay adaptable to future patches.
        /// </summary>
        public static Dictionary<uint,uint> ResearchRequiredResearch = new Dictionary<uint,uint> {
            // Terran
            { BlizzardConstants.Research.InfantryWeapons2, BlizzardConstants.Research.InfantryWeapons1},
            { BlizzardConstants.Research.InfantryWeapons3, BlizzardConstants.Research.InfantryWeapons2},
            { BlizzardConstants.Research.InfantryArmor2, BlizzardConstants.Research.InfantryArmor1},
            { BlizzardConstants.Research.InfantryArmor3, BlizzardConstants.Research.InfantryArmor2},
            // Protoss
            { BlizzardConstants.Research.GroundWeapons2, BlizzardConstants.Research.GroundWeapons1},
            { BlizzardConstants.Research.GroundWeapons3, BlizzardConstants.Research.GroundWeapons2},
            { BlizzardConstants.Research.Shields2, BlizzardConstants.Research.Shields1},
            { BlizzardConstants.Research.Shields3, BlizzardConstants.Research.Shields2},
            { BlizzardConstants.Research.AirWeapons2, BlizzardConstants.Research.AirWeapons1},
            { BlizzardConstants.Research.AirWeapons3, BlizzardConstants.Research.AirWeapons2},
            { BlizzardConstants.Research.AirArmor2, BlizzardConstants.Research.AirArmor1},
            { BlizzardConstants.Research.AirArmor3, BlizzardConstants.Research.AirArmor2},
            // Zerg - Lair
            { BlizzardConstants.Research.MeleeAttacks2, BlizzardConstants.Research.MeleeAttacks1},
            { BlizzardConstants.Research.MeleeAttacks3, BlizzardConstants.Research.MeleeAttacks2},
            { BlizzardConstants.Research.MissileAttacks2, BlizzardConstants.Research.MissileAttacks1},
            { BlizzardConstants.Research.MissileAttacks3, BlizzardConstants.Research.MissileAttacks2},
            { BlizzardConstants.Research.FlyerAttack2, BlizzardConstants.Research.FlyerAttack1},
            { BlizzardConstants.Research.FlyerAttack3, BlizzardConstants.Research.FlyerAttack2},
            { BlizzardConstants.Research.GroundCarapace2, BlizzardConstants.Research.GroundCarapace1},
            { BlizzardConstants.Research.GroundCarapace3, BlizzardConstants.Research.GroundCarapace2},
            { BlizzardConstants.Research.FlyerCarapace2, BlizzardConstants.Research.FlyerCarapace1},
            { BlizzardConstants.Research.FlyerCarapace3, BlizzardConstants.Research.FlyerCarapace2},
        };
    }
}
