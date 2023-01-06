/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using Savage_Editor.GameDev;
using System.Windows;
using System.Windows.Controls;

namespace Savage_Editor.Editors
{
	/// <summary>
	/// Interaction logic for WorldEditorView.xaml
	/// </summary>
	public partial class WorldEditorView : UserControl
	{
		public WorldEditorView()
		{
			InitializeComponent();
			Loaded += OnWorldEditorViewLoaded; // Do this when loaded
		}

		private void OnWorldEditorViewLoaded(object sender, RoutedEventArgs e)
		{
			Loaded -= OnWorldEditorViewLoaded;
			Focus(); // Set focus
		}

		private void OnNewScript_Button_Click(object sender, RoutedEventArgs e)
		{
			new NewScriptDialog().ShowDialog();
		}
	}
}
