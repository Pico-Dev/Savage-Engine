﻿/*
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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Savage_Editor.GameProject
{
	public partial class ProjectBrowserDialog : Window
	{
		// Define easing function
		private readonly CubicEase _easing = new CubicEase() { EasingMode = EasingMode.EaseInOut };
		public ProjectBrowserDialog()
		{
			InitializeComponent();
			Loaded += OnProjectBrowserLoaded;
		}

		private void OnProjectBrowserLoaded(object sender, RoutedEventArgs e)
		{
			Loaded -= OnProjectBrowserLoaded;
			if (!OpenProject.Projects.Any())
			{
				openProjectButton.IsEnabled = false;
				openProjectButton.Visibility = Visibility.Hidden;
				OnToggleButton_Click(createProjectButton, new RoutedEventArgs());
			}
		}

		// Define animation to play when selecting create project
		private void AnimateToNewProject()
		{
			var highlightAnimation = new DoubleAnimation(200, 400, new Duration(TimeSpan.FromSeconds(0.2)));
			highlightAnimation.EasingFunction = _easing;
			highlightAnimation.Completed += (s, e) =>
			{
				var animation = new ThicknessAnimation(new Thickness(0), new Thickness(-1600, 0, 0, 0), new Duration(TimeSpan.FromSeconds(0.5)));
				animation.EasingFunction = _easing;
				browserContent.BeginAnimation(MarginProperty, animation);
			};
			highlightRect.BeginAnimation(Canvas.LeftProperty, highlightAnimation);
		}

		// Define animation to play when selecting open project
		private void AnimateToOpenProject()
		{
			var highlightAnimation = new DoubleAnimation(400, 200, new Duration(TimeSpan.FromSeconds(0.2)));
			highlightAnimation.EasingFunction = _easing;
			highlightAnimation.Completed += (s, e) =>
			{
				var animation = new ThicknessAnimation(new Thickness(-1600, 0, 0, 0), new Thickness(0), new Duration(TimeSpan.FromSeconds(0.5)));
				animation.EasingFunction = _easing;
				browserContent.BeginAnimation(MarginProperty, animation);
			};
			highlightRect.BeginAnimation(Canvas.LeftProperty, highlightAnimation);
		}

		private void OnToggleButton_Click(object sendor, RoutedEventArgs e)
		{
			if (sendor == openProjectButton) // Check if sender is open project
			{
				if(createProjectButton.IsChecked == true) // Check if on create project
				{
					createProjectButton.IsChecked = false;
					AnimateToOpenProject();
					openProjectView.IsEnabled = true;
					newProjectView.IsEnabled = false;
				}
				openProjectButton.IsChecked = true; // do nothing
			}
			else // Sender should be create project
			{
				if (openProjectButton.IsChecked == true) // Check if on open project
				{
					openProjectButton.IsChecked = false;
					AnimateToNewProject();
					newProjectView.IsEnabled = true;
					openProjectView.IsEnabled = false;
				}
				createProjectButton.IsChecked = true; // do nothing
			}
		}
	}
}