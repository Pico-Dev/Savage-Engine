/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using Savage_Editor.Components;
using Savage_Editor.GameProject;
using Savage_Editor.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Savage_Editor.Editors
{
	public class NullableBoolToBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is bool b && b == true;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is bool b && b == true;
		}
	}
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
			_propertyName = string.Empty;
			_undoAction = GetRenameAction();
		}

		private void OnName_TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (_propertyName == nameof(MSEntity.Name) && _undoAction != null)
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

		private void OnAddComponent_Button_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
		{
			var menu = FindResource("addComponentMenu") as ContextMenu;
			var btn = sender as ToggleButton;
			btn.IsChecked = true;
			menu.Placement = PlacementMode.Bottom;
			menu.PlacementTarget = btn;
			menu.MinWidth = btn.ActualWidth;
			menu.IsOpen = true;
		}

		private void AddComponent(ComponentType componentType, object data)
		{
			var creationFunction = ComponentFactory.GetCreationFunction(componentType); // Get the creation function
			var changedEntities = new List<(GameEntity entity, Component component)>(); // List of game entities and components for undo redo actions
			var vm = DataContext as MSEntity;
			// for each selected entity
			foreach (var entity in vm.SelectedEntities)
			{
				// Create the component that is of the type
				var component = creationFunction(entity, data);
				// Then try to add it
				if (entity.AddComponet(component))
				{
					// Remember what changed
					changedEntities.Add((entity, component));
				}
			}

			// Setup undo redo action if something changed
			if (changedEntities.Any())
			{
				vm.Refresh();

				Project.UndoRedo.Add(new UndoRedoAction(
					() =>
					{  // Undo
						changedEntities.ForEach(x => x.entity.RemoveComponet(x.component));
						(DataContext as MSEntity).Refresh();
					},
					() =>
					{  // Redo
						changedEntities.ForEach(x => x.entity.AddComponet(x.component));
						(DataContext as MSEntity).Refresh();
					}, // Log
					$"Add {componentType} component"));
			}
		}

		private void OnAddScriptComponent(object sender, RoutedEventArgs e)
		{
			AddComponent(ComponentType.Script, (sender as MenuItem).Header.ToString());
		}
	}
}
