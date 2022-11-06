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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Savage_Editor.GameProject
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
		private readonly string _templatePath = @"..\..\Savage-Editor\ProjectTemplates\"; // TODO: get path from the install location
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
					ValidateProjectPath();
					OnPropertyChanged(nameof(ProjectName)); // Treigger Change
				}
			}
		}

		private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\Savage-Engine\"; // Get location of my documents
		// Set public Path var
		public string ProjectPath
		{
			get => _projectPath;
			set
			{
				if (_projectPath != value)
				{
					_projectPath = value;
					ValidateProjectPath();
					OnPropertyChanged(nameof(ProjectPath)); // Trigger change
				}
			}
		}

		private bool _isValid;

		// Bool for if path is valid
		public bool IsValid
		{
			get => _isValid;
			set
			{
				if (_isValid != value)
				{
					_isValid = value;
					OnPropertyChanged(nameof(IsValid));
				}
			}
		}

		// Error mesage string
		private string _errorMsg;
		public string ErrorMsg
		{
			get => _errorMsg;
			set
			{
				if (_errorMsg != value)
				{
					_errorMsg = value;
					OnPropertyChanged(nameof(ErrorMsg));
				}
			}
		}

		private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
		public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates
		{ get; }

		private bool ValidateProjectPath()
		{
			var path = ProjectPath;
			if (!Path.EndsInDirectorySeparator(path)) path += @"\"; // Look for separator and appand it if not there
			path += $@"{ProjectName}\"; // Make sepeate folder for the project

			IsValid = false;
			if (string.IsNullOrWhiteSpace(ProjectName.Trim())) // Look if there is no name
			{
				ErrorMsg = "ERROR: Must Type in a Project Name.";
			}
			else if (ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1) // Look for invalid characters in name
			{
				ErrorMsg = "ERROR: Invalid Character(s) In Project name.";
			}
			else if (string.IsNullOrWhiteSpace(ProjectPath.Trim())) // Look if there is no path
			{
				ErrorMsg = "ERROR: Must Have a Project Path.";
			}
			else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1) // Look for invalid characters in path
			{
				ErrorMsg = "ERROR: Invalid File Path.";
			}
			else if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any()) // Look for if path is already exists and has stuff in it
			{
				ErrorMsg = "ERROR: Selected Folder is not Empty";
			}
			else
			{
				ErrorMsg = String.Empty;
				IsValid = true;
			}

			return IsValid;
		}

		public string CreateProject(ProjectTemplate template)
		{
			ValidateProjectPath(); // Validate path last time
			if (!IsValid)
			{
				return string.Empty;
			}

			if (!Path.EndsInDirectorySeparator(ProjectPath)) ProjectPath += @"\"; // Look for separator and appand it if not there
			var path = $@"{ProjectPath}{ProjectName}\"; // Make sepeate folder for the project

			try
			{
				if(!Directory.Exists(path)) Directory.CreateDirectory(path); // Create path if it does not exist
				// Create all needed sub-direcories
				foreach (var folder in template.Folders)
				{
					Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), folder)));
				}
				// Set the .Savage folder to hidden
				var dirInfo = new DirectoryInfo(path + @".Savage");
				dirInfo.Attributes |= FileAttributes.Hidden;
				// Coppy respective PNG files to new project form the template
				File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Icon.png")));
				File.Copy(template.ScreenshotFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Screenshot.png")));

				var projectXml = File.ReadAllText(template.ProjectFilePath); // Read the template
				projectXml = string.Format(projectXml, ProjectName, ProjectPath);
				var projectPath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}")); // Construct project path
				File.WriteAllText(projectPath, projectXml);
				 
				return path;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				Logger.Log(MessageType.Error, $"Failed to create {ProjectName}");
				throw;
			}
		}

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
				ValidateProjectPath();
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
				Logger.Log(MessageType.Error, $"Failed to read project templates");
				throw;
			}
		}
	}

}
