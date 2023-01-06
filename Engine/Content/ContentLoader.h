/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

#pragma once
#include "CommonHeaders.h"

#if !defined(SHIPPING)
namespace savage::content {
	bool load_game();
	void unload_game();
}
#endif // !defined(SHIPPING)