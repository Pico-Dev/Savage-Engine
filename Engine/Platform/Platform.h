/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

#pragma once
#include "CommonHeaders.h"
#include "Window.h"

namespace savage::platform {

	struct window_init_info;

	// Allow for window creation even without an init-info bu using defaults
	Window create_window(const window_init_info* const init_info = nullptr);
	// Remove the window with the id
	void remove_window(window_id id);
}