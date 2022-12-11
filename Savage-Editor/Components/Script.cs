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
using System.Runtime.Serialization;
using System.Text;

namespace Savage_Editor.Components
{
	[DataContract]
	// Single-Select
	class Script : Component
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
					OnPropertyChanged(nameof(Name));
				}
			}
		}

		public override IMSComponent GetMultiselectionComponent(MSEntity msEntity) => new MSScript(msEntity);

		public Script(GameEntity owner) : base(owner) { }
	}

	// Multi-select
	sealed class MSScript : MSComponent<Script>
	{
		private string _name;
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

		protected override bool UpdateComponents(string propertyName)
		{
			// Propagate Changes from the MS component to the selected component
			if (propertyName == nameof(Name))
			{
				SelectedComponents.ForEach(c => c.Name = _name);
				return true;
			}
			return false;
		}

		protected override bool UpdateMSComponents()
		{
			// Get information from the selected component
			Name = MSEntity.GetMixedValue(SelectedComponents, new Func<Script, string>(x => x.Name));
			return true;
		}

		public MSScript(MSEntity msEntity) : base(msEntity)
		{
			Refresh();
		}
	}
}
