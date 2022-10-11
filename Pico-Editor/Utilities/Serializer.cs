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
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Pico_Editor.Utilities
{
	public static class Serializer
	{
		public static void ToFile<T>(T instance, string path) // Write to a file
		{
			try
			{
				using var fs = new FileStream(path, FileMode.Create); // Make file
				var serializer = new DataContractSerializer(typeof(T));
				serializer.WriteObject(fs, instance);
			}
			catch (Exception ex)
			{

				Debug.WriteLine(ex.Message); // TODO: make a proper log system
			}
		}

		internal static T FromFile<T>(string path)
		{
			try
			{
				using var fs = new FileStream(path, FileMode.Open); // Read file
				var serializer = new DataContractSerializer(typeof(T));
				T instance = (T)serializer.ReadObject(fs);
				return instance;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message); // TODO: make a proper log system
				return default(T);
			}
		}
	}
}
