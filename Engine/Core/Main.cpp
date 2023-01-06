/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

#ifdef _WIN64
#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif // !WIN32_LEAN_AND_MEAN

#include <Windows.h>
#include <crtdbg.h>

extern bool engine_intialize();
extern void engine_update();
extern void engine_shutdown();

#ifndef USE_WITH_EDITOR

int WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR, int)
{
#if _DEBUG
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF); // Look for memory leaks and set a flag
#endif
	// Try an initialize the engine then if that works we update the state of the engine until we close the game
	if (engine_intialize())
	{
		MSG msg{};
		bool is_running{ true };
		// This loop calls the engine update function
		while (is_running)
		{
			// This loop reads, removes and, dispatches until there are none left to process
			while (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
			{
				TranslateMessage(&msg);
				DispatchMessage(&msg);
				// Check for the quit flag then set is running to false to stop the app
				is_running &= (msg.message != WM_QUIT);
			}

			engine_update();
		}
		engine_shutdown();
		return 0;
	}
}

#endif // !USE_WITH_EDITOR
#endif // _WIN64