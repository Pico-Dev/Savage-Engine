/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using Savage_Editor.Components;
using Savage_Editor.GameProject;
using Savage_Editor.Utilities;
using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Savage_Editor.Editors
{
	/// <summary>
	/// Interaction logic for TransformView.xaml
	/// </summary>
	public partial class TransformView : UserControl
	{
		private Action _undoAction = null;
		private bool _propertyChanged = false;
		public TransformView()
		{
			InitializeComponent();
			Loaded += OnTransformViewLoaded;
		}

		private void OnTransformViewLoaded(object sender, RoutedEventArgs e)
		{
			Loaded -= OnTransformViewLoaded;
			// Find when property view model has changed
			(DataContext as MSTransform).PropertyChanged += (s, e) => _propertyChanged = true;
		}

		// Generic action constructor to save on copy paste
		private Action GetAction(Func<Transform, (Transform transform, Vector3)> selector, Action<(Transform transform, Vector3)> forEachAction)
		{
			// must be valid data context
			if (!(DataContext is MSTransform vm))
			{
				_undoAction = null;
				_propertyChanged = false;
				return null;
			}
			var selection = vm.SelectedComponents.Select(selector).ToList(); // Make list of transforms and current position values
			return new Action(() =>
			{
				selection.ForEach(forEachAction); // Set them back to their old values
				(GameEntityView.Instance.DataContext as MSEntity)?.GetMSComponent<MSTransform>().Refresh(); // Refresh the MSTransform component 
			});
		}

		// Position Action Definition 
		private Action GetPositionAction() => GetAction((x) => (x, x.Position), (x) => x.transform.Position = x.Item2);

		// Rotation Action Definition
		private Action GetRotationAction() => GetAction((x) => (x, x.Rotation), (x) => x.transform.Rotation = x.Item2);

		// Scale Action Definition
		private Action GetScaleAction() => GetAction((x) => (x, x.Scale), (x) => x.transform.Scale = x.Item2);

		// Generic void to record actions to save on copy paste
		private void RecordActions(Action redoAction, string name)
		{
			if (_propertyChanged)
			{
				Debug.Assert(_undoAction != null);
				_propertyChanged = false;
				// Add actions to UndoRedo manager
				Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, name));
			}
		}

		private void OnPosition_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
		{
			_propertyChanged = false;
			_undoAction = GetPositionAction();
		}

		private void OnPosition_VectorBox_PreviewMouse_LBU(object sender, MouseButtonEventArgs e)
		{
			RecordActions(GetPositionAction(), "Position Changed");
		}

		private void OnRotation_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
		{
			_propertyChanged = false;
			_undoAction = GetRotationAction();
		}

		private void OnRotation_VectorBox_PreviewMouse_LBU(object sender, MouseButtonEventArgs e)
		{
			RecordActions(GetRotationAction(), "Rotation Changed");
		}

		private void OnScale_VectorBox_PreviewMouse_LBD(object sender, MouseButtonEventArgs e)
		{
			_propertyChanged = false;
			_undoAction = GetScaleAction();
		}

		private void OnScale_VectorBox_PreviewMouse_LBU(object sender, MouseButtonEventArgs e)
		{
			RecordActions(GetScaleAction(), "Scale Changed");
		}

		private void OnPosition_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			// UndoRedo action for direct input
			if (_propertyChanged && _undoAction != null)
			{
				OnPosition_VectorBox_PreviewMouse_LBU(sender, null);
			}
		}

		private void OnRotation_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			// UndoRedo action for direct input
			if (_propertyChanged && _undoAction != null)
			{
				OnRotation_VectorBox_PreviewMouse_LBU(sender, null);
			}
		}

		private void OnScale_VectorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			// UndoRedo action for direct input
			if (_propertyChanged && _undoAction != null)
			{
				OnScale_VectorBox_PreviewMouse_LBU(sender, null);
			}
		}
	}
}
