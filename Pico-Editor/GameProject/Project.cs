/*
	MIT License

Copyright (c) 2022        Daniel McLarty
Copyright (c) 2020-2022   Arash Khatami

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using Pico_Editor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Pico_Editor.GameProject
{
	[DataContract(Name = "Game")]
	class Project : ViewModelBase
	{
		public static string Extension { get; } = ".pico";
		[DataMember]
		public string Name { get; private set; } = "New Project";
		[DataMember]
		public string Path { get; private set; }

		public string FullPath => $@"{Path}{Name}\{Name}{Extension}";

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

		public ICommand UndoCommand { get; private set; }
		public ICommand RedoCommand { get; private set; }

		public ICommand AddSceneCommand { get; private set; }
		public ICommand RemoveSceneCommand { get; private set; }
		public ICommand SaveCommand { get; private set; }

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
			UndoRedo.Reset();
		}
		public static void save(Project project) // Save
		{
			Serializer.ToFile(project, project.FullPath);
			Logger.Log(MessageType.Info, $"Project saved to {project.FullPath}");
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (_scenes != null)
			{
				Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
				OnPropertyChanged(nameof(Scenes)); // Updates bindings
			}
			ActiveScene = Scenes.FirstOrDefault(x => x.IsActive); // Find active scene

			//Define add scene
			AddSceneCommand = new RelayCommand<object>(x =>
			{
				AddScene($"New Scene {_scenes.Count}"); // Make the scene
				var newScene = _scenes.Last(); // Remember the last scene
				var sceneIndex = _scenes.Count - 1; // Remember the inde of last scene

				UndoRedo.Add(new UndoRedoAction(
					() => RemoveScene(newScene), // Remove the scene
					() => _scenes.Insert(sceneIndex, newScene), // Readd the scene at the same index
					$"Add {newScene.Name}")); // Name of the action
			});

			RemoveSceneCommand = new RelayCommand<Scene>(x =>
			{
				var sceneIndex = _scenes.IndexOf(x); // Get the scene index
				RemoveScene(x); // Remove the scene

				UndoRedo.Add(new UndoRedoAction(
					() => _scenes.Insert(sceneIndex, x), // Readd the scene at the same index
					() => RemoveScene(x), // Remove the scene again,
					$"Remove {x.Name}")); // Name of the action
			}, x => !x.IsActive); // Look if scene is active

			UndoCommand = new RelayCommand<object>(x => UndoRedo.Undo()); // Setup undo command
			RedoCommand = new RelayCommand<object>(x => UndoRedo.Redo()); // Setup redo command
			SaveCommand = new RelayCommand<object>(x => save(this)); // Setup save command
		}


		public Project(string name, string path)
		{
			Name = name;
			Path = path;

			OnDeserialized(new StreamingContext());
		}
	}
}
