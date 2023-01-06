/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using System;
using System.IO;
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

		// Save the script name in binary 
		public override void WriteToBinary(BinaryWriter bw)
		{
			var nameBytes = Encoding.UTF8.GetBytes(Name);
			bw.Write(nameBytes.Length);
			bw.Write(nameBytes);
		}

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
