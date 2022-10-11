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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Pico_Editor.GameProject
{
	[DataContract]
	public class ProjectTemplate // Template project handling
	{
		[DataMember]
		public string ProjectType { get; set; } // Name of template
		[DataMember]
		public string ProjectFile { get; set; } // Name of game project
		[DataMember]
		public List<string> Folders { get; set; } // Name of engine folders

		public byte[] Icon { get; set; }
		public byte[] Screenshot { get; set; }
		public string IconFilePath { get; set; }
		public string ScreenshotFilePath { get; set; }
		public string ProjectFilePath { get; set; }
	}

	class NewProject : ViewModelBase
	{
		private readonly string _templatePath = @"..\..\Pico-Editor\ProjectTemplates\"; // TODO: get path from the install location
		private string _projectName = "NewProject"; // Set default name
		// Set public Name var
		public string ProjectName
		{
			get	=> _projectName;
			set
			{
				if(_projectName != value)
				{
					_projectName = value;
					OnPropertyChanged(nameof(ProjectName)); // Treigger Change
				}
			}
		}

		private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\Pico-Engine\"; // Get location of my documents
		// Set public Path var
		public string ProjectPath
		{
			get => _projectPath;
			set
			{
				if (_projectPath != value)
				{
					_projectPath = value;
					OnPropertyChanged(nameof(ProjectPath)); // Trigger change
				}
			}
		}

		private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
		public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates
		{ get; }

		public NewProject()
		{
			ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);
			try
			{
				var templateFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories); //Find all template.xml files for the templates
				Debug.Assert(templateFiles.Any());
				foreach (var file in templateFiles) // Loop thourgh all templates
				{
					var template = Serializer.FromFile<ProjectTemplate>(file); // Deserialize the xml files 
					template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Icon.png")); // Get the template icon
					template.Icon = File.ReadAllBytes(template.IconFilePath); // Read the icon data form the file
					template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Screenshot.png")); // Get the template screenshot
					template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath); // Read the screenshot data from the file
					template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile)); // Get the project file path
					_projectTemplates.Add(template);
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
				// TODO: Make proper log system
			}
		}
	}

}
