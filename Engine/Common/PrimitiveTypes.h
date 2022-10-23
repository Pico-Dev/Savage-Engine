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

#pragma once
#include <stdint.h>

// Unsigned integers

using u64 = uint64_t;
using u32 = uint32_t;
using u16 = uint16_t;
using u8  = uint8_t;

// Signed integers

using s64 = int64_t;
using s32 = int32_t;
using s16 = int16_t;
using s8  = int8_t;

// Set invalid value to -1
constexpr u64 u64_invalid_id { 0xffff'ffff'ffff'ffffui64 };
constexpr u32 u32_invalid_id { 0xffff'ffffui32 };
constexpr u16 u16_invalid_id { 0xffffui16 };
constexpr u8  u8_invalid_id  { 0xffui8 };

// Floats
using f32 = float;