/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Savage_Editor.Dictionaries
{
	public partial class ControlTemplates : ResourceDictionary
	{
		// Define key down event for text boxes
		private void OnTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			var textBox = sender as TextBox; // Get text box reference
			var exp = textBox.GetBindingExpression(TextBox.TextProperty);
			if (exp == null) return; // Cant do anything

			// Take new value
			if (e.Key == Key.Enter)
			{
				if (textBox.Tag is ICommand command && command.CanExecute(textBox.Text))
				{
					command.Execute(textBox.Text);
				}
				else
				{
					exp.UpdateSource();
				}
				Keyboard.ClearFocus();
				e.Handled = true;
			}
			// Keep old value
			else if (e.Key == Key.Escape)
			{
				exp.UpdateTarget();
				Keyboard.ClearFocus();
			}
		}

		// Define key down event for text boxes with rename
		private void OnTextBoxRename_KeyDown(object sender, KeyEventArgs e)
		{
			var textBox = sender as TextBox; // Get text box reference
			var exp = textBox.GetBindingExpression(TextBox.TextProperty);
			if (exp == null) return; // Cant do anything

			// Take new value
			if (e.Key == Key.Enter)
			{
				if (textBox.Tag is ICommand command && command.CanExecute(textBox.Text))
				{
					command.Execute(textBox.Text);
				}
				else
				{
					exp.UpdateSource();
				}
				textBox.Visibility = Visibility.Collapsed; // Hide after being changed
				e.Handled = true;
			}
			// Keep old value
			else if (e.Key == Key.Escape)
			{
				exp.UpdateTarget();
				textBox.Visibility = Visibility.Collapsed; // Hide after being changed
			}
		}

		// Define lost focus event for text boxes with rename
		private void OnTextBoxRename_LostFocus(object sender, RoutedEventArgs e)
		{
			var textBox = sender as TextBox; // Get text box reference
			if (!textBox.IsVisible) return;
			var exp = textBox.GetBindingExpression(TextBox.TextProperty);
			if (exp != null) // Cant be null
			{
				exp.UpdateTarget(); // go back to old value
				textBox.Visibility = Visibility.Collapsed; // Hide after losing focus
			}
		}

		// Define click event for close buttons
		private void OnClose_Button_Click(object sender, RoutedEventArgs e)
		{
			var window = (Window)((FrameworkElement)sender).TemplatedParent;
			window.Close();
		}

		// Define click event for restore buttons
		private void OnMazimizeRestore_Button_Click(object sender, RoutedEventArgs e)
		{
			var window = (Window)((FrameworkElement)sender).TemplatedParent;
			window.WindowState = (window.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
		}

		// Define click event for minimize buttons
		private void OnMinimize_Button_Click(object sender, RoutedEventArgs e)
		{
			var window = (Window)((FrameworkElement)sender).TemplatedParent;
			window.WindowState = WindowState.Minimized;
		}
	}
}
