/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using Savage_Editor.Components;
using Savage_Editor.DLLWrappers;
using Savage_Editor.GameDev;
using Savage_Editor.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Savage_Editor.GameProject
{
	enum BuildConfiguration
	{
		Debug,
		DebugEditor,
		Release,
		ReleaseEditor,
	}

	[DataContract(Name = "Game")]
	class Project : ViewModelBase
	{
		public static string Extension { get; } = ".savage";
		[DataMember]
		public string Name { get; private set; } = "New Project";
		[DataMember]
		public string Path { get; private set; }
		public string FullPath => $@"{Path}{Name}{Extension}";
		public string Solution => $@"{Path}{Name}.sln";

		private static readonly string[] _buildConfigurationNames = new string[] { "Debug", "DebugEditor", "Release", "ReleaseEditor" };

		private int _buildConfig;
		[DataMember]
		public int BuildConfig
		{
			get => _buildConfig;
			set
			{
				if (_buildConfig != value)
				{
					_buildConfig = value;
					OnPropertyChanged(nameof(BuildConfig));
				}
			}
		}

		public BuildConfiguration StandAloneBiuldConfig => BuildConfig == 0 ? BuildConfiguration.Debug : BuildConfiguration.Release;
		public BuildConfiguration DLLBiuldConfig => BuildConfig == 0 ? BuildConfiguration.DebugEditor : BuildConfiguration.ReleaseEditor;

		private string[] _availableScripts;
		public string[] AvailableScripts
		{
			get => _availableScripts;
			set
			{
				if (_availableScripts != value)
				{
					_availableScripts = value;
					OnPropertyChanged(nameof(AvailableScripts));
				}
			}
		}

		[DataMember(Name = "Scenes")]
		private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
		public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }

		private Scene _activeScene;
		public Scene ActiveScene
		{
			get => _activeScene;
			set
			{
				if (_activeScene != value)
				{
					_activeScene = value;
					OnPropertyChanged(nameof(ActiveScene));
				}
			}
		}
		public static Project Current => Application.Current.MainWindow.DataContext as Project;

		public static UndoRedo UndoRedo { get; } = new UndoRedo();

		public ICommand UndoCommand { get; private set; }           // Undo an action
		public ICommand RedoCommand { get; private set; }           // Redo an action
		public ICommand AddSceneCommand { get; private set; }       // Add a scene
		public ICommand RemoveSceneCommand { get; private set; }    // Remove a scene
		public ICommand SaveCommand { get; private set; }           // Save the project
		public ICommand DebugCommand { get; private set; }          // Start the game with a debugger
		public ICommand WithoutDebugCommand { get; private set; }   // Start the game without a debugger
		public ICommand DebugStopCommand { get; private set; }      // Stop the debugger
		public ICommand BuildCommand { get; private set; }          // Build the game

		private void SetCommands()
		{
			//Define add scene
			AddSceneCommand = new RelayCommand<object>(x =>
			{
				AddScene($"New Scene {_scenes.Count}"); // Make the scene
				var newScene = _scenes.Last(); // Remember the last scene
				var sceneIndex = _scenes.Count - 1; // Remember the index of last scene

				UndoRedo.Add(new UndoRedoAction(
					() => RemoveScene(newScene), // Remove the scene
					() => _scenes.Insert(sceneIndex, newScene), // Re-add the scene at the same index
					$"Add {newScene.Name}")); // Name of the action
			});

			RemoveSceneCommand = new RelayCommand<Scene>(x =>
			{
				var sceneIndex = _scenes.IndexOf(x); // Get the scene index
				RemoveScene(x); // Remove the scene

				UndoRedo.Add(new UndoRedoAction(
					() => _scenes.Insert(sceneIndex, x), // Re-add the scene at the same index
					() => RemoveScene(x), // Remove the scene again,
					$"Remove {x.Name}")); // Name of the action
			}, x => !x.IsActive); // Look if scene is active

			UndoCommand = new RelayCommand<object>(x => UndoRedo.Undo(), x => UndoRedo.UndoList.Any());                                             // Setup undo command
			RedoCommand = new RelayCommand<object>(x => UndoRedo.Redo(), x => UndoRedo.RedoList.Any());                                             // Setup redo command
			SaveCommand = new RelayCommand<object>(x => save(this));                                                                                // Setup save command
			DebugCommand = new RelayCommand<object>(async x => await RunGame(true), x => !VisualStudio.IsDebugging() && VisualStudio.BuildDone);        // Setup debug command
			WithoutDebugCommand = new RelayCommand<object>(async x => await RunGame(false), x => !VisualStudio.IsDebugging() && VisualStudio.BuildDone);    // Setup without debug command
			DebugStopCommand = new RelayCommand<object>(async x => await StopGame(), x => VisualStudio.IsDebugging());                                  // Setup debug stop command
			BuildCommand = new RelayCommand<bool>(async x => await BuildGameCodeDLL(), x => !VisualStudio.IsDebugging() && VisualStudio.BuildDone); // Setup build command

			// Notify UI of command initialization
			OnPropertyChanged(nameof(AddSceneCommand));
			OnPropertyChanged(nameof(RemoveSceneCommand));
			OnPropertyChanged(nameof(UndoCommand));
			OnPropertyChanged(nameof(RedoCommand));
			OnPropertyChanged(nameof(SaveCommand));
			OnPropertyChanged(nameof(DebugCommand));
			OnPropertyChanged(nameof(WithoutDebugCommand));
			OnPropertyChanged(nameof(DebugStopCommand));
			OnPropertyChanged(nameof(BuildCommand));
		}

		private static string _getConfigurationNames(BuildConfiguration config) => _buildConfigurationNames[(int)config];

		private void AddScene(string sceneName)
		{
			Debug.Assert(!string.IsNullOrEmpty(sceneName.Trim()));
			_scenes.Add(new Scene(this, sceneName)); // Add the scene to the private var
		}

		private void RemoveScene(Scene scene)
		{
			Debug.Assert(_scenes.Contains(scene));
			_scenes.Remove(scene); // Remove the scene
		}

		public static Project Load(string file) // Load
		{
			Debug.Assert(File.Exists(file));
			return Serializer.FromFile<Project>(file);
		}

		public void Unload()
		{
			UnloadGameCodeDLL();
			VisualStudio.CloseVisualStudio();
			UndoRedo.Reset();
		}

		// Save the project such that it works with the engine
		private void SaveToBinary()
		{
			var configName = _getConfigurationNames(StandAloneBiuldConfig);
			var bin = $@"{Path}x64\{configName}\game.bin";

			using (var bw = new BinaryWriter(File.Open(bin, FileMode.Create, FileAccess.Write)))
			{
				bw.Write(ActiveScene.GameEntities.Count);
				foreach (var entity in ActiveScene.GameEntities)
				{
					bw.Write(0); // Entity type (reserved for later)
					bw.Write(entity.Components.Count); // Number of components in the entity
													   // Write all components
					foreach (var component in entity.Components)
					{
						bw.Write((int)component.ToEnumType());
						component.WriteToBinary(bw);
					}
				}
			}
		}

		// Build and run the game code
		private async Task RunGame(bool debug)
		{
			var configName = _getConfigurationNames(StandAloneBiuldConfig);
			// Build the solution on another thread
			await Task.Run(() => VisualStudio.BuildSolution(this, configName, debug));
			// Try and run the game code on another thread for responsiveness
			if (VisualStudio.BuildSucceeded)
			{
				SaveToBinary();
				await Task.Run(() => VisualStudio.Run(this, configName, debug));
			}
		}

		// Stop the game code
		private async Task StopGame() => await Task.Run(() => VisualStudio.Stop());

		private async Task BuildGameCodeDLL(bool showWindow = true)
		{
			try
			{
				UnloadGameCodeDLL();
				// Build then load the DLL
				await Task.Run(() => VisualStudio.BuildSolution(this, _getConfigurationNames(DLLBiuldConfig), showWindow));
				if (VisualStudio.BuildSucceeded)
				{
					LoadGameCodeDLL();
				}
			}
			catch (Exception ex)
			{
				// Throw an error if something went wrong
				Debug.WriteLine(ex.Message);
				Logger.Log(MessageType.Error, "Failed to compile project");
				throw;
			}
		}

		private void LoadGameCodeDLL()
		{
			// Find where the DLL is located for the config
			var configName = _getConfigurationNames(DLLBiuldConfig);
			var dll = $@"{Path}x64\{configName}\{Name}.dll";

			AvailableScripts = null;

			// Load the DLL
			// Broken Code | if(File.Exists(dll && EngineAPI.LoadGameCodeDLL(dll) != 0))
			if (File.Exists(dll))
			{
				EngineAPI.LoadGameCodeDLL(dll);
				// Find the scripts
				AvailableScripts = EngineAPI.GetScriptNames();
				// Load the scripts
				ActiveScene.GameEntities.Where(x => x.GetComponent<Script>() != null).ToList().ForEach(x => x.IsActive = true);
				Logger.Log(MessageType.Info, "Game Code DLL loaded successfully.");
			}
			else // If the DLL fails to load
			{
				Logger.Log(MessageType.Warning, "Game Code DLL failed load. Try rebuilding the project.");
			}
		}

		private void UnloadGameCodeDLL()
		{
			// Unload the scripts
			ActiveScene.GameEntities.Where(x => x.GetComponent<Script>() != null).ToList().ForEach(x => x.IsActive = false);
			// Unload the DLL
			if (EngineAPI.UnloadGameCodeDLL() != 0)
			{
				Logger.Log(MessageType.Info, "Game Code DLL unloaded.");
				AvailableScripts = null;
			}
		}

		public static void save(Project project) // Save
		{
			Serializer.ToFile(project, project.FullPath);
			Logger.Log(MessageType.Info, $"Project saved to {project.FullPath}");
		}

		[OnDeserialized]
		private async void OnDeserialized(StreamingContext context)
		{
			if (_scenes != null)
			{
				Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
				OnPropertyChanged(nameof(Scenes)); // Updates bindings
			}
			ActiveScene = Scenes.FirstOrDefault(x => x.IsActive); // Find active scene
			Debug.Assert(ActiveScene != null);

			await BuildGameCodeDLL(false); // Build the game code

			SetCommands();
		}


		public Project(string name, string path)
		{
			Name = name;
			Path = path;

			OnDeserialized(new StreamingContext());
		}
	}
}
