/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

#include "Common.h"
#include "CommonHeaders.h"
#include "..\Engine\Components\Script.h"
#include "..\Graphics\Renderer.h"
#include "..\Platform\PlatformTypes.h"
#include "..\Platform\Platform.h"

#ifndef WIN32_MEAN_AND_LEAN
#define WIN32_MEAN_AND_LEAN
#endif

#include <Windows.h>

using namespace savage;

namespace {
	HMODULE game_code_dll{ nullptr };
	using _get_script_creator = savage::script::detail::script_creator(*)(size_t);
	_get_script_creator get_script_creator{ nullptr };
	using _get_script_names = LPSAFEARRAY(*)(void);
	_get_script_names get_script_names{ nullptr };

	// Array of render surfaces
	utl::vector<graphics::render_surface> surfaces;

} // Anonymous namespace

EDITOR_INTERFACE u32 LoadGameCodeDLL(const char* dll_path)
{
	if (game_code_dll) return FALSE; // Return false if already loaded
	game_code_dll = LoadLibraryA(dll_path); // Load the DLL
	assert(game_code_dll);
	
	// Get a pointer to the get script names then ask for it
	get_script_names = (_get_script_names)GetProcAddress(game_code_dll, "get_script_names");
	// Do the same for script creator 
	get_script_creator = (_get_script_creator)GetProcAddress(game_code_dll, "get_sctipt_creator");

	// Return the state
	return (game_code_dll && get_script_creator && get_script_names) ? TRUE : FALSE;
}

EDITOR_INTERFACE u32 UnloadGameCodeDLL(const char* dll_path)
{
	if (!game_code_dll) return FALSE; // Return false if already unloaded
	assert(game_code_dll);
	int result{ FreeLibrary(game_code_dll) }; // Free the DLL
	assert(result); // Should be unloaded now
	game_code_dll = nullptr;
	return TRUE;
}

EDITOR_INTERFACE script::detail::script_creator GetScriptCreator(const char* name)
{
	// If the DLL is loaded AND we have the script creator call it then convert the name to a tag then return the pointer otherwise it is a null pointer
	return (game_code_dll && get_script_creator) ? get_script_creator(script::detail::string_hash()(name)) : nullptr;
}

EDITOR_INTERFACE LPSAFEARRAY GetScriptNames()
{
	// If the DLL is loaded AND we have the script names return them otherwise return a null pointer
	return (game_code_dll && get_script_names) ? get_script_names() : nullptr;
}

EDITOR_INTERFACE u32 CreateRenderSurface(HWND host, s32 width, s32 height)
{
	assert(host);
	// Create the window init info
	platform::window_init_info info{ nullptr, host, nullptr, 0, 0, width, height };
	// Create the window
	graphics::render_surface surface{ platform::create_window(&info), {} };
	assert(surface.window.is_valid());
	surfaces.emplace_back(surface);
	return (u32)surfaces.size() - 1;
}

EDITOR_INTERFACE void RemoveRenderSurface(u32 id)
{
	assert(id < surfaces.size());
	platform::remove_window(surfaces[id].window.get_id());
}

EDITOR_INTERFACE HWND GetWindowHandle(u32 id)
{
	assert(id < surfaces.size());
	return (HWND)surfaces[id].window.handle();
}

EDITOR_INTERFACE void ResizeRenderSurface(u32 id)
{
	assert(id < surfaces.size());
	// The size does not matter as we just are updating the client area so I had some fun sue me
	surfaces[id].window.resize(69, 420);
}