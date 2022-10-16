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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pico_Editor.Dictionaries
{
	public partial class ControlTemplates : ResourceDictionary
	{
		private void OnTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			var textBox = sender as TextBox; // Get textbox refrence
			var exp = textBox.GetBindingExpression(TextBox.TextProperty);
			if (exp == null) return; // Cant do anything

			// Take new value
			if(e.Key == Key.Enter)
			{
				if(textBox.Tag is ICommand command && command.CanExecute(textBox.Text))
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
			else if(e.Key == Key.Escape)
			{
				exp.UpdateTarget();
				Keyboard.ClearFocus();
			}
		}
	}
}
