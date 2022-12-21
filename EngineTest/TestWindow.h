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

#include "Test.h"
#include "..\Platform\PlatformTypes.h"
#include "..\Platform\Platform.h"

using namespace savage;

platform::Window _windows[4];

LRESULT win_proc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
{
	switch (msg)
	{
	case WM_DESTROY:
	{
		bool all_closed{ true };
		// Look for any open window if not found quit the program
		for (u32 i{ 0 }; i < _countof(_windows); i++)
		{
			if (!_windows[i].is_closed())
			{
				all_closed = false;
			}
		}
		if (all_closed)
		{
			PostQuitMessage(0);
			return(0);
		}
	}
	break;
	case WM_SYSCHAR:
		if (wparam == VK_RETURN && (HIWORD(lparam) & KF_ALTDOWN))
		{
			platform::Window win{ platform::window_id{(id::id_type)GetWindowLongPtr(hwnd, GWLP_USERDATA)} };
			win.set_fullscreen(!win.is_fullscreen());
			return 0;
		}
		break;
	}

	return DefWindowProc(hwnd, msg, wparam, lparam);
}

class engine_test : public test
{
public:
	bool initialize() override
	{

		platform::window_init_info info[]
		{
			{&win_proc, nullptr, L"Test Window 1", 100, 100, 400, 800},
			{&win_proc, nullptr, L"Test Window 2", 150, 150, 400, 800},
			{&win_proc, nullptr, L"Test Window 3", 200, 200, 400, 800},
			{&win_proc, nullptr, L"Test Window 4", 250, 250, 400, 800},
		};
		static_assert(_countof(info) == _countof(_windows));

		// Create the windows
		for (u32 i{ 0 }; i < _countof(_windows); i++) _windows[i] = platform::create_window(&info[i]);
		return true;
	}

	void run() override
	{
		std::this_thread::sleep_for(std::chrono::microseconds(10));
	}

	void shutdown() override
	{
		// Remove the window
		for (u32 i{ 0 }; i < _countof(_windows); i++) platform::remove_window(_windows[i].get_id());
	}
};