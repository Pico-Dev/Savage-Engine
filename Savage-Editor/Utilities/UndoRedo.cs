/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/


// General code-base wide undo redo system

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Savage_Editor.Utilities
{
	public interface IUndoRedo // Make an interface
	{
		string Name { get; }
		void Undo();
		void Redo();
	}

	public class UndoRedoAction : IUndoRedo
	{
		private Action _undoAction;
		private Action _redoAction;

		public string Name { get; }

		public void Redo() => _redoAction();

		public void Undo() => _undoAction();

		public UndoRedoAction(string name)
		{
			Name = name;
		}

		// Basic undo redo constructor
		public UndoRedoAction(Action undo, Action redo, string name)
			: this(name)
		{
			Debug.Assert(undo != null && redo != null);
			_undoAction = undo;
			_redoAction = redo;
		}

		// Different undo redo constructor
		public UndoRedoAction(string property, object instance, object undoValue, object redoValue, string name) :
			this(
				() => instance.GetType().GetProperty(property).SetValue(instance, undoValue),
				() => instance.GetType().GetProperty(property).SetValue(instance, redoValue),
				name)
		{ }
	}

	public class UndoRedo
	{
		private bool _enableAdd = true;
		private readonly ObservableCollection<IUndoRedo> _redoList = new ObservableCollection<IUndoRedo>();
		private readonly ObservableCollection<IUndoRedo> _undoList = new ObservableCollection<IUndoRedo>();
		public ReadOnlyObservableCollection<IUndoRedo> RedoList { get; }
		public ReadOnlyObservableCollection<IUndoRedo> UndoList { get; }

		// Clear the redo undo history
		public void Reset()
		{
			_redoList.Clear();
			_undoList.Clear();
		}

		public void Add(IUndoRedo cmd)
		{
			if (_enableAdd)
			{
				_undoList.Add(cmd);
				_redoList.Clear();
			}
		}

		public void Undo()
		{
			if (_undoList.Any()) // Look for any undo commands
			{
				var cmd = _undoList.Last(); // Find the most recent thing
				_undoList.RemoveAt(_undoList.Count - 1); // Remove the Command form the list
				_enableAdd = false; // Lock actions
				cmd.Undo();
				_enableAdd = true; // Unlock
				_redoList.Insert(0, cmd); // Add it to the top of the redo list
			}
		}

		public void Redo()
		{
			if (_redoList.Any())
			{
				var cmd = _redoList.First(); // Find the most recent thing
				_redoList.RemoveAt(0); // Remove the Command form the list
				_enableAdd = false; // Lock actions
				cmd.Redo();
				_enableAdd = true; // Unlock
				_undoList.Add(cmd); // Add it to the top of the undo list
			}
		}

		public UndoRedo()
		{
			// Set the list to their respective types
			RedoList = new ReadOnlyObservableCollection<IUndoRedo>(_redoList);
			UndoList = new ReadOnlyObservableCollection<IUndoRedo>(_undoList);
		}
	}
}
