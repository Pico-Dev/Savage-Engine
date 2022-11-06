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

using Savage_Editor.Components;
using Savage_Editor.EngineAPIStructs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Savage_Editor.EngineAPIStructs
{
	[StructLayout(LayoutKind.Sequential)]
	class TransformComponent
	{
		public Vector3 Position;
		public Vector3 Rotation;
		public Vector3 Scale = new Vector3(1, 1, 1);
	}

	[StructLayout(LayoutKind.Sequential)]
	class GameEntityDescriptor
	{
		public TransformComponent Transform = new TransformComponent();
	}
} // Anoymous namespace

namespace Savage_Editor.DLLWrappers
{
	static class EngineAPI
	{
		private const string _dllName = "EngineDLL.dll";

		// Convert editor entity to engine entity
		[DllImport(_dllName)]
		private static extern int CreateGameEntity(GameEntityDescriptor desc);
		public static int CreateGameEntity(GameEntity entity)
		{
			GameEntityDescriptor desc = new GameEntityDescriptor();

			// Transform component
			{
				var c = entity.GetComponent<Transform>();
				desc.Transform.Position = c.Position;
				desc.Transform.Rotation = c.Rotation;
				desc.Transform.Scale = c.Scale;
			}

			return CreateGameEntity(desc);
		}

		[DllImport(_dllName)]
		private static extern void RemoveGameEntity(int id);
		public static void RemoveGameEntity(GameEntity entity)
		{
			RemoveGameEntity(entity.EntityID);
		}
	}
}
