/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Savage_Editor
{
	[DataContract(IsReference = true)]
	public class ViewModelBase : INotifyPropertyChanged // Public class for implementing INotify
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName) // Handels changed proprieties
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // Invokes an event
		}
	}
}
