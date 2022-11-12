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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;
using Savage_Editor.Components;
using Savage_Editor.GameProject;
using Savage_Editor.Utilities;
using System.ComponentModel;

namespace Savage_Editor.Editors
{
	/// <summary>
	/// Interaction logic for GameEntityView.xaml
	/// </summary>
	public partial class GameEntityView : UserControl
	{
		private Action _undoAction;
		private string _propertyName;
		public static GameEntityView Instance { get; private set; }
		public GameEntityView()
		{
			InitializeComponent();
			DataContext = null;
			Instance = this;
			DataContextChanged += (_, __) =>
			{
				if (DataContext != null)
				{
					(DataContext as MSEntity).PropertyChanged += (s, e) => _propertyName = e.PropertyName;
				}
			};

		}

		private Action GetRenameAction()
		{
			var vm = DataContext as MSEntity; // Remember entities old name
			var selection = vm.SelectedEntities.Select(entity => (entity, entity.Name)).ToList(); // Remember entities new name 
			return new Action(() =>
			{
				selection.ForEach(item => item.entity.Name = item.Name);
				(DataContext as MSEntity).Refresh();
			});
		}

		private Action GetIsEnabledAction()
		{
			var vm = DataContext as MSEntity; // Remember entities old name
			var selection = vm.SelectedEntities.Select(entity => (entity, entity.IsEnbaled)).ToList(); // Remember entities new name 
			return new Action(() =>
			{
				selection.ForEach(item => item.entity.IsEnbaled = item.IsEnbaled);
				(DataContext as MSEntity).Refresh();
			});
		}

		private void OnName_TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			_propertyName = string.Empty();
			_undoAction = GetRenameAction();
		}

		private void OnName_TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if(_propertyName == nameof(MSEntity.Name) && _undoAction != null)
			{
				var redoAction = GetRenameAction();
				Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, "Rename game entity / game entities"));
				_propertyName = null;
			}
			_undoAction = null;
		}

		private void OnIsEnabled_CheckBox_Click(object sender, RoutedEventArgs e)
		{
			var undoAction = GetIsEnabledAction(); // Remember old state
			var vm = DataContext as MSEntity;
			vm.IsEnbaled = (sender as CheckBox).IsChecked == true;
			var redoAction = GetIsEnabledAction(); // Remember new values
			Project.UndoRedo.Add(new UndoRedoAction(undoAction, redoAction, vm.IsEnbaled == true ? "Enable game entity / game entities" : "Disable game entity / game entities"));
		}
	}
}
