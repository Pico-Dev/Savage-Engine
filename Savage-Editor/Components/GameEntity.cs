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


using Savage_Editor.GameProject;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using Savage_Editor.Utilities;
using Savage_Editor.DLLWrappers;

namespace Savage_Editor.Components
{
	[DataContract]
	[KnownType(typeof(Transform))]
	[KnownType(typeof(Script))]
	class GameEntity : ViewModelBase
	{
		private int _entityID = ID.INVALID_ID;
		public int EntityID
		{
			get => _entityID;
			set
			{
				if (_entityID != value)
				{
					_entityID = value;
					OnPropertyChanged(nameof(_entityID));
				}
			}
		}

		private bool _isActive;
		public bool IsActive
		{
			get => _isActive;
			set
			{
				if (_isActive != value)
				{
					_isActive = value;
					if(_isActive) // Should be loaded
					{
						EntityID = EngineAPI.EntityAPI.CreateGameEntity(this);
						Debug.Assert(ID.IsValid(_entityID));
					}
					else if(ID.IsValid(_entityID))// Should be removed
					{
						EngineAPI.EntityAPI.RemoveGameEntity(this);
						EntityID = ID.INVALID_ID;
					}
					OnPropertyChanged(nameof(IsActive));
				}
			}
		}

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

		public Component GetComponent(Type type) => Components.FirstOrDefault(c => c.GetType() == type);
		public T GetComponent<T>() where T : Component => GetComponent(typeof(T)) as T;

		public bool AddComponet(Component component)
		{
			// Cant be null
			Debug.Assert(component != null);
			// Should not already exist
			if (!Components.Any(x => x.GetType() == Components.GetType()))
			{
				// Add the component
				IsActive = false;
				_components.Add(component);
				IsActive = true;
				return true;
			}
			Logger.Log(MessageType.Warning, $"Entity {Name} already has a {component.GetType().Name} component.");
			return false;			
		}

		public void RemoveComponet(Component component)
		{
			// Cant be null
			Debug.Assert(component != null);
			if (component is Transform) return; // Transform Cant be removed

			if(_components.Contains(component))
			{
				// Remove the component
				IsActive = false;
				_components.Remove(component);
				IsActive = true;
			}
		}

		[OnDeserialized]
		void OnDeserialized(StreamingContext contex)
		{
			if(_components != null)
			{
				Components = new ReadOnlyObservableCollection<Component>(_components);
				OnPropertyChanged(nameof(Components));
			}
		}

		public GameEntity(Scene scene)
		{
			Debug.Assert(scene != null); // It should never be null ever
			ParentScene = scene; // Get internal reference
			_components.Add(new Transform(this));
			OnDeserialized(new StreamingContext());
		}
	}

	// Multi-selection
	abstract class MSEntity : ViewModelBase
	{
		private bool _enableUpdates = true; // Enables update to selected entities
		private bool? _isEnbaled;
		[DataMember]
		public bool? IsEnbaled
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

		private readonly ObservableCollection<IMSComponent> _components = new ObservableCollection<IMSComponent>();
		public ReadOnlyObservableCollection<IMSComponent> Components { get; }

		public T GetMSComponent<T>() where T : IMSComponent
		{
			return (T)Components.FirstOrDefault(x => x.GetType() == typeof(T));
		}

		public List<GameEntity> SelectedEntities { get; }

		private void MakeComponentList()
		{
			_components.Clear(); // CLear the list
			var firstEntity = SelectedEntities.FirstOrDefault();
			if (firstEntity == null) return;

			foreach (var component in firstEntity.Components) // Go through all components in the entity
			{
				var type = component.GetType(); // Get it's type
				// See if all selected entities have the same component
				if(!SelectedEntities.Skip(1).Any(entity => entity.GetComponent(type) == null))
				{
					// Should not be in the list already
					Debug.Assert(Components.FirstOrDefault(x => x.GetType() == type) == null);
					_components.Add(component.GetMultiselectionComponent(this)); // Add the component to the list
				}
			}
		}

		public static float? GetMixedValue<T>(List<T> objects, Func<T, float> getProperty)
		{
			var value = getProperty(objects.First()); // Get property of first entity
			return objects.Skip(1).Any(x => !getProperty(x).IsTheSameAs(value)) ? (float?)null : value;
		}

		public static bool? GetMixedValue<T>(List<T> objects, Func<T, bool> getProperty)
		{
			var value = getProperty(objects.First()); // Get property of first entity
			return objects.Skip(1).Any(x => value != getProperty(x)) ? (bool?)null : value;
		}

		public static string GetMixedValue<T>(List<T> objects, Func<T, string> getProperty)
		{
			var value = getProperty(objects.First()); // Get property of first entity
			return objects.Skip(1).Any(x => value != getProperty(x)) ? null : value;
		}
		protected virtual bool UpdateGameEntities(string propertyName)
		{
			switch (propertyName)
			{
				case nameof(IsEnbaled): SelectedEntities.ForEach(x => x.IsEnbaled = IsEnbaled.Value); return true; // Update all values for IsEnabled
				case nameof(Name): SelectedEntities.ForEach(x => x.Name = Name); return true; // Update all values for Name
			}
			return false;
		}

		protected virtual bool UpdateMSGameEntities()
		{
			IsEnbaled = GetMixedValue(SelectedEntities, new Func<GameEntity, bool>(x => x.IsEnbaled));
			Name = GetMixedValue(SelectedEntities, new Func<GameEntity, string>(x => x.Name));

			return true;
		}

		public void Refresh()
		{
			_enableUpdates = false;
			UpdateMSGameEntities();
			MakeComponentList();
			_enableUpdates = true;
		}

		public MSEntity(List<GameEntity> entities)
		{
			Debug.Assert(entities?.Any() == true); // Can't be null or empty
			Components = new ReadOnlyObservableCollection<IMSComponent>(_components);
			SelectedEntities = entities;
			PropertyChanged += (s, e) => { if(_enableUpdates) UpdateGameEntities(e.PropertyName); }; // Update proprieties of all game entities that were changed
		}
	}

	class MSGameEntity : MSEntity
	{
		public MSGameEntity(List<GameEntity> entities) : base(entities)
		{
			Refresh();
		}
	}
}
