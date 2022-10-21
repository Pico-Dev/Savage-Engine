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

using Pico_Editor.Components;
using Pico_Editor.GameProject;
using Pico_Editor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
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


namespace Pico_Editor.Editors
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
			vm.AddGameEntityCommand.Execute(new GameEntity(vm) { Name = "Empty Game Entity"}); // Add game entity
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
