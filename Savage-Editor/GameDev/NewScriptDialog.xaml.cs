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

using Savage_Editor.GameProject;
using Savage_Editor.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Savage_Editor.GameDev
{
	/// <summary>
	/// Interaction logic for NewScriptDialog.xaml
	/// </summary>
	public partial class NewScriptDialog : Window
	{
		private static readonly string _cppCode =
@"// Auto-generated CPP file for Savage Engine
#include ""{0}.h""

namespace {1} {{

	REGISTER_SCRIPT({0});
	void {0}::begin_play()
	{{
		// Happens once when created
	}}

	void {0}::update(float dt)
	{{
		// Happens every frame
	}}
}} // namespace {1}";

		private static readonly string _hCode =
@"// Auto-generated H file for Savage Engine
#pragma once

namespace {1} {{
	
	class {0} : public savage::script::entity_script
	{{
	public:
		constexpr explicit {0}(savage::game_entity::entity entity) : savage::script::entity_script{{entity}} {{}}

		void begin_play() override;
		void update(float dt) override;
	private:
	}};

}} // namespace {1}";

		private static readonly string _namespace = GetNamespaceFromProjectName();

		// Create the namespace name by replacing white space with underscores
		private static string GetNamespaceFromProjectName()
		{
			var projectName = Project.Current.Name;
			projectName = projectName.Replace(' ', '_');
			return projectName;
		}

		// Used to validate the name and path of the script
		// Use the text in errorMsg to see what that code is looking for
		private bool Validate()
		{
			bool isValid = false;
			var name = scriptName.Text.Trim();
			var path = scriptName.Text.Trim();
			string errorMsg = string.Empty;
			if (string.IsNullOrEmpty(name))
			{
				errorMsg = "Script name cannot be empty.";
			}
			else if (name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1 || name.Any(x => char.IsWhiteSpace(x)))
			{
				errorMsg = "Invalid character(s) used in script name.";
			}
			else if (string.IsNullOrEmpty(path))
			{
				errorMsg = "Please select a valid script path.";
			}
			else if (path.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
			{
				errorMsg = "Invalid character(s) used in script path.";
			}
			else if (Path.GetFullPath(Path.Combine(Project.Current.Path, path)).Contains(Path.Combine(Project.Current.Path, @"GameCode\")))
			{
				errorMsg = "Script must be added to (a sub-folder of) GameCode.";
			}
			else if (File.Exists(Path.GetFullPath(Path.Combine(Path.Combine(Project.Current.Path, path), $"{name}.cpp"))) ||
					File.Exists(Path.GetFullPath(Path.Combine(Path.Combine(Project.Current.Path, path), $"{name}.h"))))
			{
				errorMsg = $"script {name} already exists at that location.";
			}
			else
			{
				isValid = true;
			}

			if(!isValid)
			{
				messageTextBlock.Foreground = FindResource("Editor.RedBrush") as Brush;
			}
			else
			{
				messageTextBlock.Foreground = FindResource("Editor.FontBrush") as Brush;
			}
			messageTextBlock.Text = errorMsg;
			return isValid;
		}

		private void OnScriptName_TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!Validate()) return;
			var name = scriptName.Text.Trim();
			messageTextBlock.Text = $"{name}.cpp and {name}.h will be added to {Project.Current.Name}";
		}

		private void OnScriptPath_TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			Validate();
		}

		// Run as async to keep UI responsive
		private async void OnCreate_Button_Click(object sender, RoutedEventArgs e)
		{
			if (!Validate()) return; // Validate one last time
			IsEnabled = false; // Disable the pop up so values don't change

			// Load the busy animation
			busyAnimation.Opacity = 0;
			busyAnimation.Visibility = Visibility.Visible;
			DoubleAnimation fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(500)));
			busyAnimation.BeginAnimation(OpacityProperty, fadeIn);

			try 
			{
				// Create the needed variables locally
				var name = scriptName.Text.Trim();
				var path = Path.GetFullPath(Path.Combine(Project.Current.Path, scriptPath.Text.Trim()));
				var solution = Project.Current.Solution;
				var projectName = Project.Current.Name;

				// Create the script
				await Task.Run(() => CreateScript(name, path, solution, projectName));
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				Logger.Log(MessageType.Error, $"Failed to create script {scriptName.Text}");
			}
			finally
			{
				// Remove the busy animation
				DoubleAnimation fadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(200)));
				fadeOut.Completed += (s, e) =>
				{
					busyAnimation.Opacity = 0;
					busyAnimation.Visibility = Visibility.Hidden;
					Close();
				};
				busyAnimation.BeginAnimation(OpacityProperty, fadeOut);
			}
		}

		private void CreateScript(string name, string path, string solution, string projectName)
		{
			// If the folder given does not exist create it
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);

			// Get the new files names
			var cpp = Path.GetFullPath(Path.Combine(path, $"{name}.cpp"));
			var h = Path.GetFullPath(Path.Combine(path, $"{name}.h"));

			// Create the CPP file from the template
			using (var sw = File.CreateText(cpp))
			{
				sw.Write(string.Format(_cppCode, name, _namespace));
			}

			// Create the H file from the template
			using (var sw = File.CreateText(h))
			{
				sw.Write(string.Format(_hCode, name, _namespace));
			}

			string[] files = new string[] { cpp, h };

			for (int i = 0; i < 3; i++)
			{
				if (!VisualStudio.AddFilesToSolution(solution, projectName, files)) System.Threading.Thread.Sleep(1000);
				else break;
			}
		}

		public NewScriptDialog()
		{
			InitializeComponent();
			Owner = Application.Current.MainWindow;
			scriptPath.Text = @"GameCode\";
		}
	}
}
