/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using System.Windows;
using System.Windows.Controls;

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
