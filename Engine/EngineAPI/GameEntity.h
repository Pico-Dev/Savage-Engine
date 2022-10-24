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

#include "..\Components\ComponentsCommon.h"
#include "TransformComponent.h"

namespace pico::game_entity {

	DEFINE_TYPED_ID(entity_id);

	class entity {
	public:
		// When making an instance of this class without a parameter for the constructor it will return an invalid ID
		constexpr explicit entity(entity_id id) : _id{ id } {}
		constexpr entity() : _id{ id::invalid_id } {}
		// If the entity has a valid ID it will return that
		constexpr entity_id get_id() const { return _id; }
		// Check if ID is valid
		constexpr bool is_valid() const { return id::is_valid(_id); }

		// Get the transform
		transform::component transform() const;
	private:
		entity_id _id;
	};
}