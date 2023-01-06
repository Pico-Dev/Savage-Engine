/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using System.Windows;
using System.Windows.Controls;

namespace Savage_Editor.GameProject
{
	/// <summary>
	/// Interaction logic for OpenProjectView.xaml
	/// </summary>
	public partial class OpenProjectView : UserControl
	{
		public OpenProjectView()
		{
			InitializeComponent();

			// Set the first item as the selected item if one exists
			Loaded += (s, e) =>
			{
				var item = projectsListBox.ItemContainerGenerator
				.ContainerFromIndex(projectsListBox.SelectedIndex) as ListBoxItem;
				item?.Focus();
			};
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{

		}

		private void OnOpen_Button_Click(object sender, RoutedEventArgs e)
		{
			OpenSelectedProject();
		}

		private void OnListBoxItem_Mouse_DoubleClick(object sender, RoutedEventArgs e)
		{
			OpenSelectedProject();
		}

		private void OpenSelectedProject()
		{
			var project = OpenProject.Open(projectsListBox.SelectedItem as ProjectData); // Load the selected project
			bool dialogResult = false;
			var win = Window.GetWindow(this);
			if (project != null) // Set if it worked or not
			{
				dialogResult = true;
				win.DataContext = project;
			}
			win.DialogResult = dialogResult;
			win.Close();
		}
	}
}
