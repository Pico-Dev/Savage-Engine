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

using Savage_Editor.Components;
using Savage_Editor.GameProject;
using Savage_Editor.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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
