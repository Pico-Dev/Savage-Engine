/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

#pragma once

#include "CommonHeaders.h"
#include "..\Platform\Window.h"

namespace savage::graphics {

	// Defines what is in the window
	class surface
	{};

	// Couples the window to the surface
	struct render_surface
	{
		platform::Window window{};
		surface surface{};
	};
}