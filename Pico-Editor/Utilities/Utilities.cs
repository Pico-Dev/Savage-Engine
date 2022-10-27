﻿/*
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
using System.Text;

namespace Pico_Editor.Utilities
{
	static public class ID
	{
		public static int INVALID_ID => -1;
		public static bool IsValid(int id) => id != INVALID_ID;
	}

	static public class MathUtil
	{
		public static float Epsilon => 0.00001f;

		// Allow for close enough becasue floats are evil
		public static bool IsTheSameAs(this float value, float other)
		{
			return Math.Abs(value - other) < Epsilon;
		}

		// Allow for close enough with nullable values becasue floats are evil
		public static bool IsTheSameAs(this float? value, float? other)
		{
			if (!value.HasValue || !other.HasValue) return false;
			return Math.Abs(value.Value - other.Value) < Epsilon;
		}
	}
}
