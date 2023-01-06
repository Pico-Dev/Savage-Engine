/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using Savage_Editor.Components;
using Savage_Editor.EngineAPIStructs;
using Savage_Editor.GameProject;
using Savage_Editor.Utilities;
using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

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
		[DllImport(_engineDLL)]
		public static extern int CreateRenderSurface(IntPtr host, int width, int height);
		[DllImport(_engineDLL)]
		public static extern int RemoveRenderSurface(int surfaceID);
		[DllImport(_engineDLL)]
		public static extern IntPtr GetWindowHandle(int surfaceID);

		[DllImport(_engineDLL)]
		public static extern int ResizeRenderSurface(int surfaceID);


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
					if (c != null && Project.Current != null) // Check for script component and that the project is loaded so it may be deferred until the DLL is loaded.
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
