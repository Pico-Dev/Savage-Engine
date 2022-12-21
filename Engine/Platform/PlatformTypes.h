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
#include "CommonHeaders.h"

#ifdef _WIN64
#ifndef WIN32_MEAN_AND_LEAN
#define WIN32_MEAN_AND_LEAN
#endif // !WIN32_MEAN_AND_LEAN
#include <Windows.h>

namespace savage::platform {
	
	// Function pointer to a signature for the window procedure to be called
	using window_proc = LRESULT(*)(HWND, UINT, WPARAM, LPARAM);
	// Window handler 
	using window_handle = HWND;

	// Define the window init info for MS Windows
	struct window_init_info
	{
		window_proc		callback	{ nullptr };
		window_handle	parent		{ nullptr }; // Needed for level editor
		const wchar_t*	caption		{ nullptr };
		s32				left		{ 0 };
		s32				top			{ 0 };
		s32				width		{ 1920 };
		s32				height		{ 1080 };
	};
}
#endif // _WIN64
