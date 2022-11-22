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
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Savage_Editor
{
	public partial class MainWindow : Window
	{
		public static string SavagePath { get; private set; }

		public MainWindow()
		{
			InitializeComponent();
			Loaded += OnMainWindowLoaded; // Create reference
			Closing += OnMainWindowClosing; // Unload project
		}


		private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
		{
			Loaded -= OnMainWindowLoaded; // Deference
			GetEnginePath();
			OpenProjectBrowserDialog(); // Open the dialog window
		}

		private void GetEnginePath()
		{
			// Get the environment variable
			var enginePath = Environment.GetEnvironmentVariable("SAVAGE_ENGINE", EnvironmentVariableTarget.User);

			// Check if the environment variable is valid or not
			if (enginePath == null || !Directory.Exists(Path.Combine(enginePath, @"Engine\EngineAPI")))
			{
				var dlg = new EnginePathDialog();
				if (dlg.ShowDialog() == true)
				{
					// Set the environment variable for the engine location
					SavagePath = dlg.SavagePath;
					Environment.SetEnvironmentVariable("SAVAGE_ENGINE", SavagePath.ToUpper(), EnvironmentVariableTarget.User);
				}
				else
				{
					Application.Current.Shutdown();
				}
			}
			else
			{
				// Set the engine path
				SavagePath = enginePath;
			}
		}

		private void OnMainWindowClosing(object sender, CancelEventArgs e)
		{
			Closing -= OnMainWindowClosing; // Deference
			Project.Current?.Unload(); // Unload the project
		}

		private void OpenProjectBrowserDialog()
		{
			var projectBrowser = new ProjectBrowserDialog(); // Create instance of project browser
			if (projectBrowser.ShowDialog() == false || projectBrowser.DataContext == null) // Test if user closes dialog window or the project is null
			{
				Application.Current.Shutdown(); // Terminate the app
			}
			else
			{
				Project.Current?.Unload(); // Unload old project if any
				DataContext = projectBrowser.DataContext; // load the project
			}
		}
	}
}
