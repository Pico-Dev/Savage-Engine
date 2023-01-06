/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using System;
using System.IO;
using System.Windows;

namespace Savage_Editor
{
	/// <summary>
	/// Interaction logic for EnginePathDialog.xaml
	/// </summary>
	public partial class EnginePathDialog : Window
	{
		public string SavagePath { get; private set; }
		public EnginePathDialog()
		{
			InitializeComponent();
			Owner = Application.Current.MainWindow;
		}

		private void OnOkay_Button_Click(object sender, RoutedEventArgs e)
		{
			var path = pathTextBox.Text.Trim();
			messageTextBlock.Text = String.Empty;

			// Check path validity
			if (string.IsNullOrEmpty(path))
			{
				messageTextBlock.Text = "Invalid path.";
			}
			else if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
			{
				messageTextBlock.Text = "Invalid character(s) used in path.";
			}
			else if (!Directory.Exists(Path.Combine(path, @"Engine\EngineAPI")))
			{
				messageTextBlock.Text = "Unable to find the engine API at that location.";
			}

			// Set the path and close
			if (string.IsNullOrEmpty(messageTextBlock.Text))
			{
				if (!Path.EndsInDirectorySeparator(path)) path += @"\";
				SavagePath = path;
				DialogResult = true;
				Close();
			}
		}
	}
}
