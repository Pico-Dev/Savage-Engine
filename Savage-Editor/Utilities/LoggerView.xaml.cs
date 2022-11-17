﻿/*
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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Savage_Editor.Utilities
{
	/// <summary>
	/// Interaction logic for LoggerView.xaml
	/// </summary>
	public partial class LoggerView : UserControl
	{
		public LoggerView()
		{
			InitializeComponent();
		}

		private void OnClear_Button_Click(object sender, RoutedEventArgs e)
		{
			Logger.Clear();
		}

		private void OnMessageFileter_Button_Click(object sender, RoutedEventArgs e)
		{
			var filter = 0x0;
			if (toggleInfo.IsChecked == true) filter |= (int)MessageType.Info;
			if (toggleWarning.IsChecked == true) filter |= (int)MessageType.Warning;
			if (toggleError.IsChecked == true) filter |= (int)MessageType.Error;
			Logger.SetMessageFilter(filter);
		}
	}
}