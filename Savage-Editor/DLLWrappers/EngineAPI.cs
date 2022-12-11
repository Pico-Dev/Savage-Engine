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
using Savage_Editor.GameProject;
using Savage_Editor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
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
	class ScriptComponent
	{
		public IntPtr ScriptCreator;
	}

	[StructLayout(LayoutKind.Sequential)]
	class GameEntityDescriptor
	{
		public TransformComponent Transform = new TransformComponent();
		public ScriptComponent Script = new ScriptComponent();
	}
} // Anonymous namespace

namespace Savage_Editor.DLLWrappers
{
	static class EngineAPI
	{
		private const string _engineDLL = "EngineDLL.dll";
		[DllImport(_engineDLL, CharSet = CharSet.Ansi)]
		public static extern int LoadGameCodeDLL(string dllPath);
		[DllImport(_engineDLL)]
		public static extern int UnloadGameCodeDLL();
		[DllImport(_engineDLL)]
		public static extern IntPtr GetScriptCreator(string name);
		[DllImport(_engineDLL)]
		[return: MarshalAs(UnmanagedType.SafeArray)]
		public static extern string[] GetScriptNames();

		internal static class EntityAPI
		{
			// Convert editor entity to engine entity
			[DllImport(_engineDLL)]
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
				// Script component
				{
					var c = entity.GetComponent<Script>();
					if(c != null && Project.Current != null) // Check for script component and that the project is loaded so it may be deferred until the DLL is loaded.
					{
						// Find the script in the project
						if (Project.Current.AvailableScripts.Contains(c.Name))
						{
							// Add the script creator
							desc.Script.ScriptCreator = GetScriptCreator(c.Name);
						}
						else
						{
							// Log an error
							Logger.Log(MessageType.Error, $"Unable to find the script {c.Name}. Script component will NOT be created.");
						}
					}
				}

				return CreateGameEntity(desc);
			}

			[DllImport(_engineDLL)]
			private static extern void RemoveGameEntity(int id);
			public static void RemoveGameEntity(GameEntity entity)
			{
				RemoveGameEntity(entity.EntityID);
			}
		}
	}
}
