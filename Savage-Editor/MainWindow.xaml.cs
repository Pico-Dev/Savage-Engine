/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/


using Savage_Editor.GameProject;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

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
