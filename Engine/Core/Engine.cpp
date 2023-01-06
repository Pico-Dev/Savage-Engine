/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

#ifndef SHIPPING

#include "..\Content\ContentLoader.h"
#include "..\Components\Script.h"
#include "..\Platform\PlatformTypes.h"
#include "..\Platform\Platform.h"
#include "..\Graphics\Renderer.h"
#include <thread>

using namespace savage;

graphics::render_surface game_window{};

namespace {
	LRESULT win_proc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
	{
		switch (msg)
		{
		case WM_DESTROY:
		{
			if (game_window.window.is_closed())
			{
				PostQuitMessage(0);
				return(0);
			}
		}
		break;
		case WM_SYSCHAR:
			if (wparam == VK_RETURN && (HIWORD(lparam) & KF_ALTDOWN))
			{
				game_window.window.set_fullscreen(!game_window.window.is_fullscreen());
				return 0;
			}
			break;
		}

		return DefWindowProc(hwnd, msg, wparam, lparam);
	}
} // Anonymous namespace

bool engine_intialize()
{
	// Load the game then return the result
	if (!content::load_game()) return false;
	
	// Set the window info
	platform::window_init_info info
	{
		&win_proc, nullptr, L"Savage Game" // TODO: Get the name from the game project
	};

	// Make the window
	game_window.window = platform::create_window(&info);
	if (!game_window.window.is_valid()) return false;

	return true;
}

void engine_update()
{
	script::update(10.f);
	// Sleep for 10ms
	std::this_thread::sleep_for(std::chrono::milliseconds(10));
}

void engine_shutdown()
{
	// Unload the game
	platform::remove_window(game_window.window.get_id());
	content::unload_game();
}

#endif // !SHIPPING
