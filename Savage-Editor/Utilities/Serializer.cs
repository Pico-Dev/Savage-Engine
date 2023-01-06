/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace Savage_Editor.Utilities
{
	public static class Serializer
	{
		// Write to a file
		public static void ToFile<T>(T instance, string path)
		{
			try
			{
				using var fs = new FileStream(path, FileMode.Create); // Make file
				var serializer = new DataContractSerializer(typeof(T));
				serializer.WriteObject(fs, instance); // Write an XML file
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				Logger.Log(MessageType.Error, $"Failed to serialize {instance} to {path}");
				throw;
			}
		}

		internal static T FromFile<T>(string path)
		{
			try
			{
				using var fs = new FileStream(path, FileMode.Open); // Read file
				var serializer = new DataContractSerializer(typeof(T));
				T instance = (T)serializer.ReadObject(fs); // Get the contents
				return instance;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				Logger.Log(MessageType.Error, $"Failed to de-serialize {path}");
				throw;
			}
		}
	}
}
