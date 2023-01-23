/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

#pragma once
#include "CommonHeaders.h"

// Keep declarations consistent and avoid name mangling by the compiler
#ifndef  EDITOR_INTERFACE
#define EDITOR_INTERFACE extern "C" __declspec(dllexport)
#endif // ! EDITOR_INTERFACE
