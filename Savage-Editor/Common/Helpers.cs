/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using System.Windows;
using System.Windows.Media;

namespace Savage_Editor
{
	static class VisualExtensions
	{
		// Finds any type of visual parent
		public static T FindVisualParent<T>(this DependencyObject depObject) where T : DependencyObject
		{
			if (!(depObject is Visual)) return null;

			// Take the parent of the dependency object if it is a visual
			var parent = VisualTreeHelper.GetParent(depObject);

			// If parent is not null check if the parent is the type we requested then return it otherwise return null
			while(parent != null)
			{
				if(parent is T type)
				{
					return type;
				}
				// Next get the parent of the parent to git to the top of the visual tree
				parent = VisualTreeHelper.GetParent(parent);
			}
			return null;
		}
	}
}
