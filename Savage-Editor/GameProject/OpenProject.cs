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

using Savage_Editor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Shapes;

namespace Savage_Editor.GameProject
{
	[DataContract]
	public class ProjectData
	{
		[DataMember]
		public string ProjectName { get; set; }
		[DataMember]
		public string ProjectPath { get; set; }
		[DataMember]
		public DateTime Date { get; set; }

		public string FullPath { get => $"{ProjectPath}{ProjectName}{Project.Extension}"; }
		public byte[] Icon { get; set; }
		public byte[] Screenshot { get; set; }
	}

	[DataContract]
	public class ProjectDataList
	{
		[DataMember]
		public List<ProjectData> Projects { get; set; }
	}

	class OpenProject
	{
		private static readonly string _applicationDataPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Savage-Engine\"; // Get the appdata folder
		private static readonly string _projectDataPath;
		private static readonly ObservableCollection<ProjectData> _projects = new ObservableCollection<ProjectData>();
		public static ReadOnlyObservableCollection<ProjectData> Projects
		{ get; }
		private static void ReadProjectData()
		{
			if (File.Exists(_projectDataPath)) // Check if project exists
			{
				var projects = Serializer.FromFile<ProjectDataList>(_projectDataPath).Projects.OrderByDescending(x => x.Date); // De-serialize the data and order it form new to old
				_projects.Clear();
				foreach (var project in projects)
				{
					if (File.Exists(project.FullPath)) // Make sure it was not deleted
					{
						// Get the Icon and Screen-shot
						project.Icon = File.ReadAllBytes($@"{project.ProjectPath}\.Savage\Icon.png");
						project.Screenshot = File.ReadAllBytes($@"{project.ProjectPath}\.Savage\Screenshot.png");
						_projects.Add(project); // Add it to the list
					}
				}
			}
		}
		private static void WriteProjectData()
		{
			var projects = _projects.OrderBy(x => x.Date).ToList();
			Serializer.ToFile(new ProjectDataList() { Projects = projects }, _projectDataPath); // Write the data file
		}

		// Return the project data
		public static Project Open(ProjectData data)
		{
			ReadProjectData();
			var project = _projects.FirstOrDefault(x => x.FullPath == data.FullPath);
			// If the project exist set the date
			if (project != null)
			{
				project.Date = DateTime.Now;
			}
			// If the project is new set the data, date and, add it to the project XML file
			else
			{
				project = data;
				project.Date = DateTime.Now;
				_projects.Add(project);
			}
			WriteProjectData();

			return Project.Load(project.FullPath);
		}


		static OpenProject() // Remember location of project
		{
			try
			{
				if (!Directory.Exists(_applicationDataPath)) Directory.CreateDirectory(_applicationDataPath); // Create appdata folder if needed
				_projectDataPath = $@"{_applicationDataPath}ProjectData.xml"; // Look for XML file
				Projects = new ReadOnlyObservableCollection<ProjectData>(_projects); // Make observable list
				ReadProjectData(); 
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				Logger.Log(MessageType.Error, $"Failed to read project data");
				throw;
			}
		}
	}
}
