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
using System.Windows.Input;

namespace Savage_Editor.Utilities.Controls
{
	[TemplatePart(Name = "PART_textBlock", Type = typeof(TextBlock))]
	[TemplatePart(Name = "PART_textBox", Type = typeof(TextBox))]
	class NumberBox : Control
	{
		// Save original value
		private double _originalValue;
		// Remember the mouse is captured
		private bool _captured = false;
		// Remember if the value has changed
		private bool _valueChanged = false;
		// Remember the starting mouse value
		private double _mouseXStart;
		// How fast the value changes
		private double _multiplier;

		// Set dependency property for the multiplier
		public double Multiplier
		{
			get => (double)GetValue(MultiplierProperty);
			set => SetValue(MultiplierProperty, value);
		}
		// Backing field for dependency property
		public static readonly DependencyProperty MultiplierProperty =
			DependencyProperty.Register(nameof(Multiplier), typeof(double), typeof(NumberBox),
			new PropertyMetadata(1.0));

		// Set dependency property for the value
		public string Value
		{
			get => (string)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}
		// Backing field for dependency property
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register(nameof(Value), typeof(string), typeof(NumberBox),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			// Define the text box overwrites for the number box
			if (GetTemplateChild("PART_textBlock") is TextBlock textBlock)
			{
				textBlock.MouseLeftButtonDown += OnTextBlock_Mouse_LBD;
				textBlock.MouseLeftButtonUp += OnTextBlock_Mouse_LBU;
				textBlock.MouseMove += OnTextBlock_Mouse_Move;
			}
		}

		// Define what to do when the user clicks
		private void OnTextBlock_Mouse_LBD(object sender, MouseButtonEventArgs e)
		{
			double.TryParse(Value, out _originalValue); // Turn the string into a real value

			Mouse.Capture(sender as UIElement); // Get the mouse
			_captured = true;
			_valueChanged = false;
			e.Handled = true;

			// Get the mouse position when we clicked
			_mouseXStart = e.GetPosition(this).X;

			Focus();
		}

		// Define what to do when the user stops clicking
		private void OnTextBlock_Mouse_LBU(object sender, MouseButtonEventArgs e)
		{
			if (_captured) // Only do stuff if the mouse is captured
			{
				Mouse.Capture(null); // Dereference the mouse
				_captured = false; // Remove the captured flag
				e.Handled = true;

				// Use keyboard for value change if we did not drag the value
				if (!_valueChanged && GetTemplateChild("PART_textBox") is TextBox textBox)
				{
					// Self explanatory setup boilerplate
					textBox.Visibility = Visibility.Visible;
					textBox.Focus();
					textBox.SelectAll();
				}
			}
		}

		// Define what to do when the mouse moves
		private void OnTextBlock_Mouse_Move(object sender, MouseEventArgs e)
		{
			if (_captured) // Only do stuff if the mouse is captured
			{
				var mouseX = e.GetPosition(this).X; // Get current mouse position
				var d = mouseX - _mouseXStart; // Get the total distance moved

				// Test if mouse has moved enough
				if (Math.Abs(d) > SystemParameters.MinimumHorizontalDragDistance)
				{
					// Set multiplier depending on shift or control key is down
					if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) _multiplier = 0.001;
					else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) _multiplier = 0.1;
					else _multiplier = 0.01;

					var newValue = _originalValue + (d * _multiplier * Multiplier); // Get the new value
					Value = newValue.ToString("0.#####"); // Get the string that we will show
					_valueChanged = true;
				}
			}
		}

		static NumberBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberBox), new FrameworkPropertyMetadata(typeof(NumberBox))); // Overwrite the default property
		}
	}
}