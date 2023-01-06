/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
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
