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

#pragma once
#include "ComponentsCommon.h"

namespace pico::transform {

	// Define typed ID for transform component
	DEFINE_TYPED_ID(transform_id);

	// Contains initilization information for transform component
	struct inti_info
	{
		f32 positon[3]{};
		f32 rotation[4]{};
		f32 scale[3]{1.f, 1.f, 1.f};
	};

	// Create transform component
	transform_id create_transform(const inti_info& info, game_entity::entity_id entity_id);
	// Remove transform component
	void remove_transform(transform_id id);
}