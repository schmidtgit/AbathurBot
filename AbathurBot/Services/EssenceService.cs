using Abathur.Constants;
using Google.Protobuf;
using NydusNetwork;
using NydusNetwork.API.Protocol;
using NydusNetwork.Logging;
using System;
using System.IO;
using System.Linq;

namespace Launcher.Services {
    /// <summary>
    /// Used to generate the 'essence' file.
    /// Essence contains information that may change between patches - but not between matches.
    /// </summary>
    class EssenceService {
        private const int TIMEOUT = 100000;

        /// <summary>
        /// Will load the essence file from the given path or attempt to fetch information from client and write to location.
        /// </summary>
        /// <param name="dataPath">Valid path for data directory used by Abathur</param>
        /// <param name="log">Optional log</param>
        /// <returns></returns>
        public static Essence LoadOrCreate(string dataPath, ILogger log = null) {
            log?.LogMessage("Checking binary essence file:");
            ValidateOrCreateBinaryFile(dataPath + "essence.data",() => FetchDataFromClient(log),log);
            return Load(dataPath + "essence.data",log);
        }

        /// <summary>
        /// Will load the essence file from the desired path (assumed stored as binary protobuf).
        /// </summary>
        /// <param name="path">Path to essence file</param>
        /// <param name="log">Optional log</param>
        /// <returns></returns>
        public static Essence Load(string path,ILogger log = null) {
            using(var stream = File.OpenRead(path)) {
                var result = Essence.Parser.ParseFrom(stream);
                log?.LogSuccess($"\tLOADED: {path}");
                return result;
            }
        }

        /// <summary>
        /// Will validate existence of file (not content) or attempt to write file.
        /// </summary>
        /// <typeparam name="T">Any object that is a protobuf message</typeparam>
        /// <param name="path">Path to validate</param>
        /// <param name="content">Function to get content in case the file does not exist</param>
        /// <param name="log">Optional log</param>
        private static void ValidateOrCreateBinaryFile<T>(string path,Func<T> content,ILogger log = null) where T : IMessage {
            try {
                if(File.Exists(path)) {
                    log?.LogSuccess($"\tFOUND: {path}");
                } else {
                    var msg = content.Invoke();
                    FileStream stream = File.Create(path);
                    msg.WriteTo(stream);
                    stream.Close();
                    log?.LogWarning($"\tCREATED: {path}");
                }
            } catch(Exception e) { log.LogError($"\tFAILED: {e.Message}"); }
        }

        /// <summary>
        /// Will launch a StarCraft II client and attempt to gather information using the DataRequest.
        /// </summary>
        /// <param name="log">Optional log</param>
        /// <returns></returns>
        private static Essence FetchDataFromClient(ILogger log = null) {
            log?.LogWarning("\tFetching data from Client (THIS MIGHT TAKE A WHILE)");
            var gameSettings = Defaults.GameSettings;
            var gc = new GameClient(gameSettings,new MultiLogger { });
            gc.Initialize(true);
            log?.LogSuccess("\tSuccessfully connected to client.");

            if(!gc.TryWaitCreateGameRequest(out var r,TIMEOUT))
                TerminateWithMessage("Failed to create game",log);

            if(r.CreateGame.Error == ResponseCreateGame.Types.Error.Unset)
                log?.LogSuccess($"\tSuccessfully created game with {gameSettings.GameMap}");
            else {
                log?.LogWarning($"\tFailed with {r.JoinGame.Error} on {gameSettings.GameMap}.");
                if(!gc.TryWaitAvailableMapsRequest(out r,TIMEOUT))
                    TerminateWithMessage("Failed to fetch available maps",log);
                else
                    gameSettings.GameMap = r.AvailableMaps.BattlenetMapNames.First();
                if(!gc.TryWaitCreateGameRequest(out r,TIMEOUT))
                    TerminateWithMessage("Failed to create game",log);
                if(r.JoinGame.Error == ResponseJoinGame.Types.Error.Unset)
                    log?.LogSuccess($"\tSuccessfully created game with {gameSettings.GameMap}");
            }

            if(!gc.TryWaitJoinGameRequest(out r,TIMEOUT))
                TerminateWithMessage("Failed to join game",log);
            log?.LogSuccess("\tSuccessfully joined game.");

            if(!gc.TryWaitPingRequest(out r,TIMEOUT))
                TerminateWithMessage("Failed to ping client",log);

            log?.LogSuccess($"\tSuccessfully pinged client - requesting databuild {r.Ping.DataBuild}");
            var dataVersion = r.Ping.DataVersion;
            var dataBuild = (int)r.Ping.DataBuild;

            if(!gc.TryWaitDataRequest(out r,TIMEOUT))
                TerminateWithMessage("Failed to receive data",log);
            log?.LogSuccess("\tSuccessfully received data.");

            log?.LogWarning("\tManipulating cost of morphed units in UnitType data!");
            var data = ManipulateMorphedUnitCost(r.Data);

            if(!gc.TryWaitLeaveGameRequest(out r,TIMEOUT))
                TerminateWithMessage("Failed to leave StarCraft II application",log);
            log?.LogSuccess("\tSuccessfully closed client.");

            return new Essence {
                DataVersion = dataVersion,
                DataBuild = dataBuild,
                Abilities = { data.Abilities },
                Buffs = { data.Buffs },
                UnitTypes = { data.Units },
                Upgrades = { data.Upgrades },
                UnitProducers = { Defaults.UnitProducers.ToMultiValuePair() },
                UnitRequiredBuildings = { Defaults.UnitRequiredBuildings.ToMultiValuePair() },
                ResearchProducer = { Defaults.ResearchProducers.ToValuePair() },
                ResearchRequiredBuildings = { Defaults.ResearchRequiredBuildings.ToValuePair() },
                ResearchRequiredResearch = { Defaults.ResearchRequiredResearch.ToValuePair() },
            };
        }

        /// <summary>
        /// UnitType Data from the StarCraft II client does not show the 'cost' of upgrading units, but the value of the unit.
        /// E.g. the 'cost' of a BroodLord (114) is shown as the combined cost of a BroodLord and a Corrupter (112) that it hatch from.
        /// This will make the ProductionManager in the Abathur Framework perform suboptimal.
        /// </summary>
        /// <param name="data">ResponseData from StarCraft II Client</param>
        /// <returns></returns>
        private static ResponseData ManipulateMorphedUnitCost(ResponseData data) {
            var original = data.Units.Clone();
            foreach(var unit in data.Units) {
                if(GameConstants.IsMorphed(unit.UnitId)) {
                    // Change the price of Zerlings, as it is not possible to order just one.
                    if(unit.UnitId == BlizzardConstants.Unit.Zergling) {
                        unit.MineralCost *= 2;
                        unit.VespeneCost *= 2;
                        unit.FoodRequired *= 2;
                    } else if(Defaults.UnitProducers.TryGetValue(unit.UnitId,out var p)) {
                        // Deduct the price of the producer (eg. price of CommandCenter (18) from Planetary Fortress (130)) to optain UPGRADE cost.
                        var producer = original.FirstOrDefault(u => u.UnitId == p.FirstOrDefault());
                        if(producer == null) continue;
                        unit.MineralCost -= producer.MineralCost;
                        unit.VespeneCost -= producer.VespeneCost;
                        unit.FoodRequired -= producer.FoodRequired;
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// Print message to log and wait for key stroke before terminating application.
        /// </summary>
        /// <param name="message">Message to print to log</param>
        /// <param name="log">Log to write to</param>
        private static void TerminateWithMessage(string message,ILogger log) {
            log?.LogError($"{message} - PRESS ANY KEY TO TERMINATE");
            Console.ReadKey();
            Environment.Exit(-1);
        }
    }
}
