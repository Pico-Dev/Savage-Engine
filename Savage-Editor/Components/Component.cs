/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/


using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Savage_Editor.Components
{
	interface IMSComponent { }

	[DataContract]
	abstract class Component : ViewModelBase
	{
		[DataMember]
		public GameEntity Owner { get; private set; }

		public abstract IMSComponent GetMultiselectionComponent(MSEntity msEntity);
		public abstract void WriteToBinary(BinaryWriter bw);

		public Component(GameEntity owner)
		{
			Debug.Assert(owner != null); // Can't be null
			Owner = owner; // Internal reference
		}
	}

	// Define how multi-selection works in the editor for components
	abstract class MSComponent<T> : ViewModelBase, IMSComponent where T : Component
	{
		// Check if we can update a component
		private bool _enableUpdates = true;
		// List of components that corresponds to the type of component from multi-selection component
		public List<T> SelectedComponents { get; }

		// Update the components
		protected abstract bool UpdateComponents(string propertyName);
		// Get the info from the components
		protected abstract bool UpdateMSComponents();

		// Control update refreshing
		public void Refresh()
		{
			_enableUpdates = false;
			UpdateMSComponents();
			_enableUpdates = true;
		}

		public MSComponent(MSEntity msEntity)
		{
			Debug.Assert(msEntity?.SelectedEntities?.Any() == true); // Look for selected entities
			SelectedComponents = msEntity.SelectedEntities.Select(entity => entity.GetComponent<T>()).ToList(); // Get component of type T into the list
			PropertyChanged += (s, e) => { if (_enableUpdates) UpdateComponents(e.PropertyName); }; // When one property changes then that property in all selected entities change
		}
	}
}
