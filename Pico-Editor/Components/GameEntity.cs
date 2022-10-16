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


using Pico_Editor.GameProject;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using System.Linq.Expressions;
using System.Windows.Input;
using Pico_Editor.Utilities;

namespace Pico_Editor.Components
{
	[DataContract]
	[KnownType(typeof(Transform))]
	public class GameEntity : ViewModelBase
	{
		private bool _isEnbaled = true;
		[DataMember]
		public bool IsEnbaled
		{
			get => _isEnbaled;
			set
			{
				if (_isEnbaled != value)
				{
					_isEnbaled = value;
					OnPropertyChanged(nameof(IsEnbaled));
				}
			}
		}

		private string _name;
		[DataMember]
		public string Name
		{
			get => _name;
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged(nameof(Name));
				}
			}
		}
		[DataMember]
		public Scene ParentScene { get; private set; }

		[DataMember(Name = nameof(Components))]
		private readonly ObservableCollection<Component> _components = new ObservableCollection<Component>();
		public ReadOnlyObservableCollection<Component> Components { get; private set; }

		public ICommand RenameCommand { get; private set; }
		public ICommand IsEnableCommand { get; private set; }

		[OnDeserialized]
		void OnDeserialized(StreamingContext contex)
		{
			if(_components != null)
			{
				Components = new ReadOnlyObservableCollection<Component>(_components);
				OnPropertyChanged(nameof(Components));
			}

			RenameCommand = new RelayCommand<string>(x =>
			{
				var oldName = _name; //Remeber old name
				Name = x;

				Project.UndoRedo.Add(new UndoRedoAction(nameof(Name), this, oldName , x, $"Renaming entity '{oldName}' to '{x}'"));
			}, x => x != _name);

			IsEnableCommand = new RelayCommand<bool>(x =>
			{
				var oldValue = _isEnbaled; //Remeber old value
				IsEnbaled = x;

				Project.UndoRedo.Add(new UndoRedoAction(nameof(IsEnbaled), this, oldValue, x, x ? $"Enable {Name}" : $"Disable {Name}"));
			});
		}

		public GameEntity(Scene scene)
		{
			Debug.Assert(scene != null); // It should never be null ever
			ParentScene = scene; // Get internal refrence
			_components.Add(new Transform(this));
			OnDeserialized(new StreamingContext());
		}
	}
}
