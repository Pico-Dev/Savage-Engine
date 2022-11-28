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

#include "Common.h"
#include "CommonHeaders.h"

#ifndef WIN32_MEAN_AND_LEAN
#define WIN32_MEAN_AND_LEAN
#endif


#include <Windows.h>

using namespace savage;

namespace {
	HMODULE game_code_dll{ nullptr };
} // Anonymous namespace

EDITOR_INTERFACE u32 
LoadGameCodeDLL(const char* dll_path)
{
	if (game_code_dll) return FALSE; // Return false if already loaded
	game_code_dll = LoadLibraryA(dll_path); // Load the DLL
	assert(game_code_dll);

	// Return the loaded state
	return game_code_dll ? TRUE : FALSE;
}

EDITOR_INTERFACE u32
UnloadGameCodeDLL(const char* dll_path)
{
	if (!game_code_dll) return FALSE; // Return false if already unloaded
	assert(game_code_dll);
	int result{ FreeLibrary(game_code_dll) }; // Free the DLL
	assert(result); // Should be unloaded now
	game_code_dll = nullptr;
	return TRUE;
}