/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using System;
using System.Diagnostics;

namespace Savage_Editor.Components
{
	enum ComponentType
	{
		Transform,
		Script,
	}

	static class ComponentFactory
	{
		// Array of function that will create the type of components when they are called
		private static readonly Func<GameEntity, object, Component>[] _function = new Func<GameEntity, object, Component>[]
		{
			(entity, data) => new Transform(entity),
			(entity, data) => new Script(entity){Name = (String)data},
		};
		// Get the function that will create the component
		public static Func<GameEntity, object, Component> GetCreationFunction(ComponentType componentType)
		{
			Debug.Assert((int)componentType < _function.Length);
			return _function[(int)componentType];
		}

		public static ComponentType ToEnumType(this Component component)
		{
			return component switch
			{
				Transform _ => ComponentType.Transform,
				Script _ => ComponentType.Script,
				_ => throw new ArgumentException("Unknown component type"),
			};
		}
	}
}
