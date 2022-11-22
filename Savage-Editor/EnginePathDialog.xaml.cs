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

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
			if(string.IsNullOrEmpty(path))
			{
				messageTextBlock.Text = "Invalid path.";
			}
			else if(path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
			{
				messageTextBlock.Text = "Invalid character(s) used in path.";
			}
			else if(!Directory.Exists(Path.Combine(path, @"Engine\EngineAPI")))
			{
				messageTextBlock.Text = "Unable to find the engine API at that location.";
			}

			// Set the path and close
			if(string.IsNullOrEmpty(messageTextBlock.Text))
			{
				if (!Path.EndsInDirectorySeparator(path)) path += @"\";
				SavagePath = path;
				DialogResult = true;
				Close();
			}
		}
	}
}
