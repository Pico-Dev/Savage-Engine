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

namespace Pico_Editor.GameProject
{
	[DataContract(Name = "Game")]
	public class Project : ViewModelBase
	{
		public static string Extension { get; } = ".pico";
		[DataMember]
		public string Name { get; private set; } = "New Project";
		[DataMember]
		public string Path { get; private set; }

		public string FullPath => $"{Path}{Name}{Extension}";

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

		public static Project Load(string file) // Load
		{
			Debug.Assert(File.Exists(file));
			return Serializer.FromFile<Project>(file);
		}

		public void Unload()
		{

		}
		public static void save(Project project) // Save
		{
			Serializer.ToFile(project, project.FullPath);
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
		}


		public Project(string name, string path)
		{
			Name = name;
			Path = path;

			OnDeserialized(new StreamingContext());
		}
	}
}
