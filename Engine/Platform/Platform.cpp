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

#include "Platform.h"
#include "PlatformTypes.h"

namespace savage::platform {

#ifdef _WIN64
	namespace {
		// Define needed default window info for MS Windows
		struct window_info
		{
			HWND	hwnd			{ nullptr };
			RECT	client_area		{ 0, 0, 1920, 1080 };
			RECT	fullscreen_area	{};
			POINT	top_left		{ 0, 0 };
			DWORD	style			{ WS_VISIBLE };
			bool	is_fullscreen	{ false };
			bool	is_closed		{ false };
		};

		// Array of window information
		utl::vector<window_info> windows;

		///////////////////////////////////////////////////////////////////
		// TODO: This part will be handled by a free-set container latter
		utl::vector<u32> available_slots;

		u32 add_to_windows(window_info info)
		{
			u32 id{ u32_invalid_id };
			if (available_slots.empty())
			{
				id = (u32)windows.size();
				windows.emplace_back(info);
			}
			else
			{
				id = available_slots.back();
				available_slots.pop_back();
				assert(id != u32_invalid_id);
				windows[id] = info;
			}
			return id;
		}

		void remove_from_windows(u32 id)
		{
			assert(id < windows.size());
			available_slots.emplace_back(id);
		}
		///////////////////////////////////////////////////////////////////

		// Get the window info from an ID
		window_info& get_from_id(window_id id)
		{
			assert(id < windows.size());
			assert(windows[id].hwnd);
			return windows[id];
		}

		window_info& get_from_handle(window_handle handle)
		{
			const window_id id{ (id::id_type)GetWindowLongPtr(handle, GWLP_USERDATA) };
			return get_from_id(id);
		}

		LRESULT CALLBACK internal_window_proc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
		{
			// Handle messages
			window_info* info{ nullptr };
			switch (msg)
			{
			case WM_DESTROY:
				get_from_handle(hwnd).is_closed = true;
				break;
			case WM_EXITSIZEMOVE:
				info = &get_from_handle(hwnd);
				break;
			case WM_SIZE:
				if (wparam == SIZE_MAXIMIZED)
				{
					info = &get_from_handle(hwnd);
				}
				break;
			case WM_SYSCOMMAND:
				if (wparam == SC_RESTORE)
				{
					info = &get_from_handle(hwnd);
				}
				break;
			default:
				break;
			}

			if (info)
			{
				assert(info->hwnd);
				GetClientRect(info->hwnd, info->is_fullscreen ? &info->fullscreen_area : &info->client_area);
			}

			LONG_PTR long_ptr{ GetWindowLongPtr(hwnd, 0) }; // Points to a buffer and at index 0 it reserves the bytes for the callback
			// Get a pointer to the callback function then if it is not null call it otherwise call the default
			return long_ptr ? ((window_proc)long_ptr)(hwnd, msg, wparam, lparam) : DefWindowProc(hwnd, msg, wparam, lparam);
		}

		void resize_window(const window_info& info, const RECT& area)
		{
			// Set the window size for the device
			RECT window_rect{ area };
			AdjustWindowRect(&window_rect, info.style, FALSE);

			const s32 width{ window_rect.right - window_rect.right };
			const s32 height{ window_rect.bottom - window_rect.top };

			MoveWindow(info.hwnd, info.top_left.x, info.top_left.y, width, height, true);
		}

		void resize_window(window_id id, u32 width, u32 height)
		{
			window_info& info{ get_from_id(id) };

			RECT& area{ info.is_fullscreen ? info.fullscreen_area : info.client_area };
			area.bottom = area.top + height;
			area.right = area.left + width;

			resize_window(info, area);
		}

		void set_window_fullscreen(window_id id, bool is_fullscreen)
		{
			// Get reference to the window info instance
			window_info& info{ get_from_id(id) };

			// Set the fullscreen status
			if (info.is_fullscreen != is_fullscreen)
			{
				info.is_fullscreen = is_fullscreen;
				if (is_fullscreen)
				{
					// Store the old window info so it can be restored latter
					GetClientRect(info.hwnd, &info.client_area);
					RECT rect;
					GetWindowRect(info.hwnd, &rect);

					// Fullscreen info
					info.top_left.x = rect.left;
					info.top_left.y = rect.top;
					info.style = 0;

					// Set the style and show the window
					SetWindowLongPtr(info.hwnd, GWL_STYLE, info.style);
					ShowWindow(info.hwnd, SW_MAXIMIZE);
				}
				else
				{
					info.style = WS_VISIBLE | WS_OVERLAPPEDWINDOW;

					// Set the style of the window
					SetWindowLongPtr(info.hwnd, GWL_STYLE, info.style);

					// Restore the old info
					resize_window(info, info.client_area);
					ShowWindow(info.hwnd, SW_SHOWNORMAL);
				}
			}
		}

		bool is_window_fullscreen(window_id id)
		{
			return get_from_id(id).is_fullscreen;
		}

		window_handle get_window_handle(window_id id)
		{
			return get_from_id(id).hwnd;
		}

		void set_window_caption(window_id id, const wchar_t* caption)
		{
			window_info& info{ get_from_id(id) };
			SetWindowText(info.hwnd, caption);
		}

		math::u32v4 get_window_size(window_id id)
		{
			window_info& info{ get_from_id(id) };
			RECT area{ info.is_fullscreen ? info.fullscreen_area : info.client_area };
			return { (u32)area.left, (u32)area.top, (u32)area.right, (u32)area.bottom, };
		}

		bool is_window_closed(window_id id)
		{
			return get_from_id(id).is_closed;
		}

	} // Anonymous namespace

	Window create_window(const window_init_info* const init_info /* = nullptr */)
	{
		// Use the value in init if it is not null
		window_proc callback{ init_info ? init_info->callback : nullptr };
		window_handle parent{ init_info ? init_info->parent : nullptr };

		// Setup a window class
		WNDCLASSEX wc;
		ZeroMemory(&wc, sizeof(wc));
		wc.cbSize = sizeof(WNDCLASSEX); // size of the structure
		wc.style = CS_HREDRAW | CS_VREDRAW; // Style of the window
		wc.lpfnWndProc = internal_window_proc; // Function pointer to an internal window procedure
		wc.cbClsExtra = 0; // Number of extra bytes allocated following the class structure
		wc.cbWndExtra = callback ? sizeof(callback) : 0; // Allocate space for the callback
		wc.hInstance = 0;
		wc.hIcon = LoadIcon(NULL, IDI_APPLICATION); // Use the default app icon
		wc.hCursor = LoadCursor(NULL, IDC_ARROW); // Use the default cursor
		wc.hbrBackground = CreateSolidBrush(RGB(26, 48, 76)); // Background color
		wc.lpszMenuName = NULL;
		wc.lpszClassName = L"SavageWindow"; // Set the class name
		wc.hIconSm = LoadIcon(NULL, IDI_APPLICATION); // Use the default small icon

		// Register a window class
		RegisterClassEx(&wc);

		window_info info{};
		RECT rc{ info.client_area }; // content of window except title bar

		// Adjust the window for the correct device size
		AdjustWindowRect(&rc, info.style, FALSE);

		const wchar_t* caption{ (init_info && init_info->caption) ? init_info->caption : L"Savage Game" };
		const s32 left{ (init_info && init_info->left) ? init_info->left : info.client_area.left };
		const s32 top { (init_info && init_info->top)  ? init_info->top  : info.client_area.top };
		const s32 width{ (init_info && init_info->width) ? init_info->width : rc.right - rc.left };
		const s32 height{ (init_info && init_info->height) ? init_info->height : rc.bottom - rc.top };

		info.style |= parent ? WS_CHILD : WS_OVERLAPPEDWINDOW;

		// Create an instant of the class
		info.hwnd = CreateWindowEx(
			0,					// Extended Style
			wc.lpszClassName,	// Window class name
			caption,			// Instance title
			info.style,			// Window Style
			left, top,			// Initial window position
			width, height,		// Initial window dimensions
			parent,				// Handle to parent 
			NULL,				// Handle to menu
			NULL,				// Instance of this application
			NULL);				// extra creation parameters

		if (info.hwnd)
		{
			// Clear any error
			SetLastError(0);
			// Set the ID and save it in a long pointer
			const window_id id{ add_to_windows(info) };
			SetWindowLongPtr(info.hwnd, GWLP_USERDATA, (LONG_PTR)id);

			// Set the callback pointer at the index if one exists
			if(callback) SetWindowLongPtr(info.hwnd, 0, (LONG_PTR)callback);
			assert(GetLastError() == 0);

			// Show the window and return the ID
			ShowWindow(info.hwnd, SW_SHOWNORMAL);
			UpdateWindow(info.hwnd);
			return Window{ id };
		}
		return {};
	}

	void remove_window(window_id id)
	{
		window_info& info{ get_from_id(id) };
		DestroyWindow(info.hwnd);
		remove_from_windows(id);
	}
#elif
#error "Must implement at least one platform"
#endif // _WIN64

	// Window value descriptions
	void Window::set_fullscreen(bool is_fullscreen) const
	{
		assert(is_valid());
		set_window_fullscreen(_id, is_fullscreen);
	}

	bool Window::is_fullscreen() const
	{
		assert(is_valid());
		return is_window_fullscreen(_id);
	}

	void* Window::handle() const
	{
		assert(is_valid());
		return get_window_handle(_id);
	}

	void Window::set_caption(const wchar_t* caption) const
	{
		assert(is_valid());
		set_window_caption(_id, caption);
	}

	const math::u32v4 Window::size() const
	{
		assert(is_valid());
		return get_window_size(_id);
	}

	void Window::resize(u32 width, u32 height) const
	{
		assert(is_valid());
		resize_window(_id, width, height);
	}

	const u32 Window::width() const
	{
		math::u32v4 s{ size() };
		return s.z - s.x;
	}

	const u32 Window::height() const
	{
		math::u32v4 s{ size() };
		return s.w - s.y;
	}

	bool Window::is_closed() const
	{
		assert(is_valid());
		return is_window_closed(_id);
	}
}