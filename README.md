# AbathurBot Setup (PROTOTYPE)
AbathurBot allows you to write your own AI using the [Abathur Framework](https://github.com/schmidtgit/Abathur) and run it.
Clone, compile and run. The launcher should handle the rest and run a demo.
See [AdequateSource.com](http://adequatesource.com/abathur) for details.

*Part of a bachelor thesis! Please fill out this [questionnaire](https://goo.gl/forms/1Z6AHqIQzgo0ZxtK2) regarding SC2 AIs*

## Launching for the first time!
    - A directory for settings and cached data (/data) will be created at the current path
    - A directory for log files will be created (/log)
    - The directory will be populated with a gamesettings.json and a setup.json
    - A StarCraft II client will launch to fetch game data from, which will be cached at /data/essence.data (SLOW)
## Turn features on and off
    The framework comes pre-packed with modules that solve regular problems.
    Turn these modules on/off by adding/removing them from the generated setup.json file, examples:
    - AutoHarvestGather (force idle workers to mine minerals and assign vespene workers)
    - AutoSupply (queue a supply depot/overlord/pulon when supply capped and at less than 200 supply)
    - AutoRestart (start a new game when the current one ends)
    
## Write your own...
    - Create a new module (at /Modules) and implement the IModule interface
    - Open the generated setup.json file and add the classname of your module
    - Launch the framework and watch it go!
# The IModule Interface
The Abathur Framework is based around modules interacting with the core functionality of the framework.
The idea is that each module should be small and sastify a simple task, eg. making sure there is enough supply or that workers are never idle.

### Access the core functionality
The core functionality in the abathur framework is accessed through 'managers' e.g. the IntelManager.
These can be accessed through dependency injection. See the /Modules/FullModule.cs

### Initialize()
Called once - after the framework has successfully connected to the StarCraft II client.
The game state as not been fetched yet, as the game is not launched yet.

### OnStart()
Called once - after the framework has succesfully connected to the StarCraft II client.

### OnStep()
Called each frame (step-mode) or as often as possible (real-time).
Not called in the initial frame.

### OnGameEnded()
Called when the game have ended, but before the framework leave the game (or restart)

# Feedback?
This framework is still in very early development! We would appreciate feedback!
Keep updated with known issues, the roadmap and more information at adequatesource.com/abathur
