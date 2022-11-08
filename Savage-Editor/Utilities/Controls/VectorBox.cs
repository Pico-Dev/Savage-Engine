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

namespace Savage_Editor.Utilities.Controls
{
	// Define number of components
	public enum VectorType
	{
		Vector2,
		Vector3,
		Vector4,
	}

	class VectorBox : Control
	{
		// Set dependency property for the vector type
		public VectorType VectorType
		{
			get => (VectorType)GetValue(VectorTypeProperty);
			set => SetValue(VectorTypeProperty, value);
		}
		// Backing field for depenency property
		public static readonly DependencyProperty VectorTypeProperty =
			DependencyProperty.Register(nameof(VectorType), typeof(VectorType), typeof(VectorBox),
			new PropertyMetadata(VectorType.Vector3));

		// Set dependency property for the orientation
		public Orientation Orientation
		{
			get => (Orientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}
		// Backing field for depenency property
		public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(VectorBox),
			new PropertyMetadata(Orientation.Horizontal));

		// Set dependency property for the multiplier
		public double Multiplier
		{
			get => (double)GetValue(MultiplierProperty);
			set => SetValue(MultiplierProperty, value);
		}
		// Backing field for depenency property
		public static readonly DependencyProperty MultiplierProperty =
			DependencyProperty.Register(nameof(Multiplier), typeof(double), typeof(VectorBox),
			new PropertyMetadata(1.0));


		// Set dependency property for the values
		public string X
		{
			get => (string)GetValue(XProperty);
			set => SetValue(XProperty, value);
		}
		// Backing field for X
		public static readonly DependencyProperty XProperty =
			DependencyProperty.Register(nameof(X), typeof(string), typeof(VectorBox),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public string Y
		{
			get => (string)GetValue(YProperty);
			set => SetValue(YProperty, value);
		}
		// Backing field for Y
		public static readonly DependencyProperty YProperty =
			DependencyProperty.Register(nameof(Y), typeof(string), typeof(VectorBox),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public string Z
		{
			get => (string)GetValue(ZProperty);
			set => SetValue(ZProperty, value);
		}
		// Backing field for Z
		public static readonly DependencyProperty ZProperty =
			DependencyProperty.Register(nameof(Z), typeof(string), typeof(VectorBox),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public string W
		{
			get => (string)GetValue(WProperty);
			set => SetValue(WProperty, value);
		}
		// Backing field for W
		public static readonly DependencyProperty WProperty =
			DependencyProperty.Register(nameof(W), typeof(string), typeof(VectorBox),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		static VectorBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(VectorBox), new FrameworkPropertyMetadata(typeof(VectorBox)));
		}
	}
}
