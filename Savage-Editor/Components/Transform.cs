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
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;

namespace Savage_Editor.Components
{
	[DataContract]
	class Transform : Component
	{
		private Vector3 _position;
		[DataMember]
		public Vector3 Position
		{
			get => _position;
			set
			{
				if (_position != value)
				{
					_position = value;
					OnPropertyChanged(nameof(Position));
				}
			}
		}

		private Vector3 _rotation;
		[DataMember]
		public Vector3 Rotation
		{
			get => _rotation;
			set
			{
				if (_rotation != value)
				{
					_rotation = value;
					OnPropertyChanged(nameof(Rotation));
				}
			}
		}

		private Vector3 _scale;
		[DataMember]
		public Vector3 Scale
		{
			get => _scale;
			set
			{
				if (_scale != value)
				{
					_scale = value;
					OnPropertyChanged(nameof(Scale));
				}
			}
		}

		public Transform(GameEntity owner) : base(owner)
		{
		}
	}
}
