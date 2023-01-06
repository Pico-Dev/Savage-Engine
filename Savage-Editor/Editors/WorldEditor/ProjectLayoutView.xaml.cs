/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using Savage_Editor.Components;
using Savage_Editor.GameProject;
using Savage_Editor.Utilities;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace Savage_Editor.Editors
{
	/// <summary>
	/// Interaction logic for ProjectLayoutView.xaml
	/// </summary>
	public partial class ProjectLayoutView : UserControl
	{
		public ProjectLayoutView()
		{
			InitializeComponent();
		}

		private void OnAddGameEntity_Button_Click(object sender, RoutedEventArgs e)
		{
			var btn = sender as Button;
			var vm = btn.DataContext as Scene; // Get Data context
			vm.AddGameEntityCommand.Execute(new GameEntity(vm) { Name = "Empty Game Entity" }); // Add game entity
		}

		private void OnGameEntities_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var listBox = sender as ListBox; // Get list box of sender			
			var newSelection = listBox.SelectedItems.Cast<GameEntity>().ToList(); // Get list of new selection
			var previousSelection = newSelection.Except(e.AddedItems.Cast<GameEntity>()).Concat(e.RemovedItems.Cast<GameEntity>()).ToList(); // Get list of old selection

			Project.UndoRedo.Add(new UndoRedoAction(
				() => // Undo Action
				{
					listBox.UnselectAll();
					previousSelection.ForEach(x => (listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem).IsSelected = true);
				},
				() => // Redo Action
				{
					listBox.UnselectAll();
					newSelection.ForEach(x => (listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem).IsSelected = true);
				},
				"Selection Changed" // Name of action
				));

			MSGameEntity msEntity = null;
			if (newSelection.Any())
			{
				msEntity = new MSGameEntity(newSelection);
			}
			GameEntityView.Instance.DataContext = msEntity;
		}
	}
}
