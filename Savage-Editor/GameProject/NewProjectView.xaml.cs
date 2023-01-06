/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using System.Windows;
using System.Windows.Controls;

namespace Savage_Editor.GameProject
{
	public partial class NewProjectView : UserControl
	{
		public NewProjectView()
		{
			InitializeComponent();
		}

		private void OnCreate_Button_Click(object sender, RoutedEventArgs e)
		{
			var vm = DataContext as NewProject;
			var projectPath = vm.CreateProject(templateListBox.SelectedItem as ProjectTemplate); // Make the selected template
			bool dialogResult = false;
			var win = Window.GetWindow(this);
			if (!string.IsNullOrEmpty(projectPath)) // Set if it worked or not
			{
				dialogResult = true;
				var project = OpenProject.Open(new ProjectData() { ProjectName = vm.ProjectName, ProjectPath = projectPath }); // Get the project that can be used in the rest of the editor
				win.DataContext = project;
			}
			win.DialogResult = dialogResult;
			win.Close();
		}
	}
}
