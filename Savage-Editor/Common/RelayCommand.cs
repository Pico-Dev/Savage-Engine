/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using System;
using System.Windows.Input;

namespace Savage_Editor
{
	class RelayCommand<T> : ICommand
	{
		private readonly Action<T> _execute; // What needs to be done
		private readonly Predicate<T> _canExecute; // What can be done

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute?.Invoke((T)parameter) ?? true; // If not null return the vale if null return true
		}

		public void Execute(object parameter)
		{
			_execute((T)parameter); // Return the value
		}

		public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute)); // If execute is null throw an exception
			_canExecute = canExecute; // This can be null

		}
	}
}
