/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using Savage_Editor.Components;
using Savage_Editor.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace Savage_Editor.GameProject
{
	[DataContract]
	class Scene : ViewModelBase
	{
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
					OnPropertyChanged(nameof(Name)); // Trigger Change
				}
			}
		}

		[DataMember]
		public Project Project { get; private set; }

		private bool _isActive;
		[DataMember]
		public bool IsActive
		{
			get => _isActive;
			set
			{
				if (_isActive != value)
				{
					_isActive = value;
					OnPropertyChanged(nameof(IsActive));
				}
			}
		}

		[DataMember(Name = nameof(GameEntities))]
		private readonly ObservableCollection<GameEntity> _gameEntities = new ObservableCollection<GameEntity>();
		public ReadOnlyObservableCollection<GameEntity> GameEntities { get; private set; }

		public ICommand AddGameEntityCommand { get; set; }
		public ICommand RemoveGameEntityCommand { get; set; }

		private void AddGameEntity(GameEntity entity, int index = -1)
		{
			Debug.Assert(!_gameEntities.Contains(entity)); // Cant be a duplicate
			entity.IsActive = IsActive; // Set the entity to active when the scene is
			if (index == -1) // Add to list if index is invalid
			{
				_gameEntities.Add(entity);
			}
			else // Insert if the index is valid
			{
				_gameEntities.Insert(index, entity);
			}
		}

		private void RemoveGameEntity(GameEntity entity)
		{
			Debug.Assert(_gameEntities.Contains(entity)); // Must exist
			entity.IsActive = false; // Disable the entity
			_gameEntities.Remove(entity);
		}


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (_gameEntities != null)
			{
				GameEntities = new ReadOnlyObservableCollection<GameEntity>(_gameEntities);
				OnPropertyChanged(nameof(GameEntity)); // Updates bindings
			}

			// Set status on load for all entities
			foreach (var entity in _gameEntities)
			{
				entity.IsActive = IsActive;
			}

			//Define add entity
			AddGameEntityCommand = new RelayCommand<GameEntity>(x =>
			{
				AddGameEntity(x); // Make the entity
				var entityIndex = _gameEntities.Count - 1; // Remember the index of last entity

				Project.UndoRedo.Add(new UndoRedoAction(
					() => RemoveGameEntity(x), // Remove the entity
					() => AddGameEntity(x, entityIndex), // Re-add the entity at the same index
					$"Add {x.Name} to {Name}")); // Name of the action
			});

			// Define remove entity
			RemoveGameEntityCommand = new RelayCommand<GameEntity>(x =>
			{
				var entityIndex = _gameEntities.IndexOf(x); // Get the entity index
				RemoveGameEntity(x); // Remove the entity

				Project.UndoRedo.Add(new UndoRedoAction(
					() => AddGameEntity(x, entityIndex), // Re-add the entity at the same index
					() => RemoveGameEntity(x), // Remove the entity again,
					$"Remove {x.Name}")); // Name of the action
			}); // Look if entity is active

		}

		public Scene(Project project, string name)
		{
			Debug.Assert(project != null);
			Project = project;
			Name = name;
			OnDeserialized(new StreamingContext());
		}
	}
}
