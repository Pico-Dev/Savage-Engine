/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Savage_Editor.Editors
{
	[ContentProperty("ComponentContent")]
	public partial class ComponentView : UserControl
	{

		public string Header
		{
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Header.
		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register(nameof(Header), typeof(string), typeof(ComponentView));



		public FrameworkElement ComponentContent
		{
			get { return (FrameworkElement)GetValue(ComponentContentProperty); }
			set { SetValue(ComponentContentProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ComponentContent.
		public static readonly DependencyProperty ComponentContentProperty =
			DependencyProperty.Register(nameof(ComponentContent), typeof(FrameworkElement), typeof(ComponentView));


		public ComponentView()
		{
			InitializeComponent();
		}
	}
}
