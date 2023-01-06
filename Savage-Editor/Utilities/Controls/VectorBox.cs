/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

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
		// Backing field for dependency property
		public static readonly DependencyProperty VectorTypeProperty =
			DependencyProperty.Register(nameof(VectorType), typeof(VectorType), typeof(VectorBox),
			new PropertyMetadata(VectorType.Vector3));

		// Set dependency property for the orientation
		public Orientation Orientation
		{
			get => (Orientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}
		// Backing field for dependency property
		public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(VectorBox),
			new PropertyMetadata(Orientation.Horizontal));

		// Set dependency property for the multiplier
		public double Multiplier
		{
			get => (double)GetValue(MultiplierProperty);
			set => SetValue(MultiplierProperty, value);
		}
		// Backing field for dependency property
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