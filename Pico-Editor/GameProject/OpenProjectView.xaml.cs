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
using System.Xml.Serialization;

namespace Pico_Editor.GameProject
{
	/// <summary>
	/// Interaction logic for OpenProjectView.xaml
	/// </summary>
	public partial class OpenProjectView : UserControl
	{
		public OpenProjectView()
		{
			InitializeComponent();
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
			}
			win.DialogResult = dialogResult;
			win.Close();
		}
	}
}
