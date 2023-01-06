/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using System.Windows;

namespace Savage_Editor.Utilities.Controls
{
	class ScalarBox : NumberBox
	{
		static ScalarBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ScalarBox), new FrameworkPropertyMetadata(typeof(ScalarBox))); // Overwrite the default property
		}
	}
}
