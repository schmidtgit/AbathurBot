using Abathur.Core;
using Abathur.Core.Combat;
using Abathur.Core.Intel;
using Abathur.Core.Production;
using Abathur.Core.Raw;
using Abathur.Modules;
using Abathur.Repositories;
using Launcher.Settings;
using Microsoft.Extensions.DependencyInjection;
using NydusNetwork;
using NydusNetwork.API.Protocol;
using NydusNetwork.Logging;
using NydusNetwork.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Abathur.Modules.Services;
using Abathur.Core.Intel.Map;
using Abathur;

namespace Launcher.Client {
    /// <summary>
    /// This class helps configure Abathur using the setting file.
    /// A regular service provider can be used as well, but this approach allows for modules to be added/removed without recompiling the code.
    /// </summary>
    internal class AbathurConfigurator {
        /// <summary>
        /// Used for manual inject of command strings for external modules
        /// </summary>
        private static Queue<string> externalStrings;

        /// <summary>
        /// Get all types in this assembly (reflection + depedency injection for easier setup)
        /// </summary>
        private static Type[] assemblyTypes = typeof(AbathurConfigurator).Assembly.GetTypes();


        /// <summary>
        /// Method for configuring the Abathur Framework using depedency injecting and reflection.
        /// </summary>
        /// <param name="essence">Patch-dependent Starcraft II data</param>
        /// <param name="gameSettings">Settings used for launching the StarCraft II client</param>
        /// <param name="log">Optional logger - useful for test and debug purposes</param>
        /// <param name="setup">Should contain the name of all modules the framework should launch with</param>
        /// <returns></returns>
        public static IAbathur Configure(Essence essence,GameSettings gameSettings,AbathurSetup setup,ILogger log = null) {
            if(log == null)
                log = new MultiLogger(); // Give a 'decoy' logger to prevent null pointer exceptions if people rely on the log.
            externalStrings = new Queue<string>();
            var sp = ConfigureServices(essence,gameSettings,setup,log);
            var abathur = (Abathur.Abathur) sp.GetService<IAbathur>();
            abathur.IsParallelized = setup.IsParallelized;
            abathur.Modules = sp.GetServices<IModule>().ToList();
            // Setup external modules with commands.
            foreach(var m in abathur.Modules)
                if (m is ExternalModule)
                    ((ExternalModule) m).Command = externalStrings.Dequeue();
            return abathur;
        }

        /// <summary>
        /// Special service provider for the Abathur framework utilizing reflection to configurate setup without recompiling.
        /// </summary>
        /// <param name="essence">Patch-dependent Starcraft II data</param>
        /// <param name="gameSettings">Settings used for launching the StarCraft II client</param>
        /// <param name="log">Optional logger - useful for test and debug purposes</param>
        /// <param name="setup">Should contain the name of all modules the framework should launch with</param>
        /// <returns></returns>
        private static IServiceProvider ConfigureServices(Essence essence,GameSettings gameSettings,AbathurSetup setup,ILogger log) {
            var collection = new ServiceCollection();
            // Add settings, data and logger.
            collection.AddSingleton(essence);
            collection.AddSingleton(gameSettings);
            collection.AddSingleton(log);

            // Add Abathur core modules
            collection.AddSingleton<IGameClient,GameClient>();
            collection.AddSingleton<IIntelManager,IntelManager>();
            collection.AddSingleton<IRawManager,RawManager>();
            collection.AddSingleton<IProductionManager,ProductionManager>();
            collection.AddSingleton<ICombatManager,CombatManager>();
            collection.AddSingleton<ISquadRepository,CombatManager>();
            collection.AddSingleton<IGameMap,GameMap>();
            collection.AddSingleton<ITechTree,TechTree>();

            // Add Abathur data repositories
            collection.AddSingleton<DataRepository,DataRepository>();
            collection.AddSingleton<IUnitTypeRepository>(x => x.GetService<DataRepository>());
            collection.AddSingleton<IUpgradeRepository>(x => x.GetService<DataRepository>());
            collection.AddSingleton<IBuffRepository>(x => x.GetService<DataRepository>());
            collection.AddSingleton<IAbilityRepository>(x => x.GetService<DataRepository>());

            // Add services used for external modules (Python)
            collection.AddSingleton<IRawManagerService,RawManagerService>();
            collection.AddSingleton<IProductionManagerService,ProductionManagerService>();
            collection.AddSingleton<ICombatManagerService,CombatManagerService>();
            collection.AddSingleton<IIntelManagerService, IntelManagerService>();

            // Add all modules inheriting from IReplaceableModule, allowing any module to access them through their constructor
            foreach(var type in GetAllImplementations<IReplaceableModule>())
                collection.AddSingleton(type,type);

            // Add class that inherits from IModule and is mentioned in the setup file
#if DEBUG
            if(setup.Modules.Count == 0)
                log?.LogWarning($"\tLauncher: Running without ANY modules.");
#endif
            foreach(string launchString in setup.Modules) {
                if(GetType<IModule>(launchString,out var implementationType)) {
                    collection.AddSingleton(typeof(IModule),implementationType);
#if DEBUG
                    log?.LogSuccess($"\tLauncher: {launchString} resolved to a valid class.");
#endif
                } else {
                    log?.LogError($"\tLauncher: Could not resolve {launchString} to a valid class."); // Python not supported yet.
                    /*
                    // Assume the file mentioned in the setup file is a command for setting up an external module
                    collection.AddSingleton<IModule, ExternalModule>();
                    externalStrings.Enqueue(launchString);
#if DEBUG
                    log?.LogWarning($"\tLauncher: [EXTERNAL] {launchString}");
#endif
                    */
                }
            }

            // Add Abathur!
            collection.AddScoped<IAbathur,Abathur.Abathur>();
            return collection.BuildServiceProvider();
        }

        /// <summary>
        /// Utilize reflection to get all implementation types of the interface T
        /// </summary>
        /// <typeparam name="T">Any interface</typeparam>
        /// <returns></returns>
        private static IEnumerable<Type> GetAllImplementations<T>() {
            var info = typeof(T).GetTypeInfo(); // Access everything in Abathur
            return info.Assembly.GetTypes().Concat(assemblyTypes) // And launcher
                .Where(x => x != typeof(T))
                .Where(x => info.IsAssignableFrom(x));
        }

        /// <summary>
        /// Utilize reflection to get a implementation type with a specific name that inherits from the interface T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool GetType<T>(string s,out Type type) {
            var info = typeof(T).GetTypeInfo(); // Access everything in Abathur
            type = info.Assembly.GetTypes().Concat(assemblyTypes) // And launcher
                .Where(x => x != typeof(T))
                .Where(x => info.IsAssignableFrom(x))
                .Where(x => x.Name == s)
                .FirstOrDefault();
            return type == null ? false : true;
        }
    }
}
