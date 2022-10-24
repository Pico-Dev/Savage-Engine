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
#include "CommonHeaders.h"

namespace pico::id {

	// Set number of bits for entity IDs
	using id_type = u32;

	namespace internal {
		// Set number of entity bits reseved for entity generations. (Number of times an entity can safely change at that index.)
		constexpr u32 generation_bits{ 8 };
		// Set number of entity bits reseved for entity index. (Max number of entities loaded at one time.)
		constexpr u32 index_bits{ sizeof(id_type) * 8 - generation_bits };
		// Mask to get only generations bits from ID
		constexpr id_type generation_mask{ (id_type{1} << generation_bits) - 1 };
		// Mask to get only idex bits from ID
		constexpr id_type index_mask{ (id_type{1} << index_bits) - 1 };
	} // Internal namespace

	// Invalid ID check mask
	constexpr id_type invalid_id{ id_type(-1) };
	// Minimum amout of deleted elements before the the engine will reuse an ID
	constexpr u32 min_deleted_elements{ 1024 };

	// Find smallest inteter type that can fit the generation bits
	using generation_type = std::conditional_t<internal::generation_bits <= 16, std::conditional_t<internal::generation_bits <= 8, u8, u16>, u32>;
	// Generation type should not have less bits than generation bits
	static_assert(sizeof(generation_type) * 8 >= internal::generation_bits);
	// Generation type should not be bigger than id type
	static_assert(sizeof(id_type) - sizeof(generation_type) > 0);
	
	// Check if ID is valid
	constexpr bool
	is_valid(id_type id)
	{
		return id != invalid_id;
	}

	// Get index part of ID
	constexpr id_type
	index(id_type id) 
	{
		// Check if index is a valid value and give it back
		id_type index{ id & internal::index_mask };
		assert(index != internal::index_mask);
		return index;
	}

	// Get generation part of ID
	constexpr id_type
	generation(id_type id)
	{
		return (id >> internal::index_bits) & internal::generation_mask;
	}

	// Increment generation bit when we make a new object at the same ID
	constexpr id_type
	new_generation(id_type id)
	{
		const id_type generation{ id::generation(id) + 1 };
		assert(generation < (((u64)1 << internal::generation_bits) -1 ));
		return index(id) | (generation << internal::index_bits);
	}

	// Differentiates between debug biuld and release biuld to force id type
#if _DEBUG
	namespace internal {
		struct id_base
		{
			constexpr explicit id_base(id_type id) : _id{ id } {}
			constexpr operator id_type() const { return _id; }
		private:
			id_type _id;
		};
	}

#define DEFINE_TYPED_ID(name)							\
	struct name final : id::internal::id_base			\
	{													\
		constexpr explicit name(id::id_type id)			\
			: id_base{ id } {}							\
		constexpr name() : id_base{ 0 } {}				\
	};													\

#else
#define DEFINE_TYPED_ID(name) using name = id::id_type;
#endif

}