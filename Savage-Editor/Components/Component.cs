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


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Savage_Editor.Components
{
	interface IMSComponent { }

	[DataContract]
	abstract class Component : ViewModelBase
	{
		public abstract IMSComponent GetMultiselectionComponent(MSEntity msEntity);
		[DataMember]
		public GameEntity Owner { get; private set; }

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
