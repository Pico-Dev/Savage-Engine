/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/
#pragma comment(lib, "Engine.lib")

#define TEST_ENTITY_COMPONENTS 0
#define TEST_WINDOW 1

#if TEST_ENTITY_COMPONENTS
#include "TestEntityComponents.h"
#elif TEST_WINDOW
#include "TestWindow.h"
#else
#error One of the tests need to be enabled
#endif

#ifdef _WIN64
#include <Windows.h>

int WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR, int)
{
#if _DEBUG
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF); // Look for memory leaks and set a flag
#endif
	engine_test test{};
	// Try an initialize the engine then if that works we update the state of the engine until we close the game
	if (test.initialize())
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
			test.run();
		}
		test.shutdown();
		return 0;
	}
}

#else
int main()
{
#if _DEBUG
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF); // Look for memory leaks and set a flag
#endif
	engine_test test{};

	if (test.initialize())
	{
		test.run();
	}

	test.shutdown();
}

#endif // _WIN64