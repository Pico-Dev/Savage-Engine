/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

#pragma once
#include "CommonHeaders.h"

namespace savage::id {

	// Set number of bits for entity IDs
	using id_type = u32;

	namespace detail {
		// Set number of entity bits reserved for entity generations. (Number of times an entity can safely change at that index.)
		constexpr u32 generation_bits{ 10 };
		// Set number of entity bits reserved for entity index. (Max number of entities loaded at one time.)
		constexpr u32 index_bits{ sizeof(id_type) * 8 - generation_bits };
		// Mask to get only generations bits from ID
		constexpr id_type generation_mask{ (id_type{1} << generation_bits) - 1 };
		// Mask to get only index bits from ID
		constexpr id_type index_mask{ (id_type{1} << index_bits) - 1 };
	} // Internal namespace

	// Invalid ID check mask
	constexpr id_type invalid_id{ id_type(-1) };
	// Minimum amount of deleted elements before the engine will reuse an ID
	constexpr u32 min_deleted_elements{ 1024 };

	// Find smallest integer type that can fit the generation bits
	using generation_type = std::conditional_t<detail::generation_bits <= 16, std::conditional_t<detail::generation_bits <= 8, u8, u16>, u32>;
	// Generation type should not have less bits than generation bits
	static_assert(sizeof(generation_type) * 8 >= detail::generation_bits);
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
		id_type index{ id & detail::index_mask };
		assert(index != detail::index_mask);
		return index;
	}

	// Get generation part of ID
	constexpr id_type
	generation(id_type id)
	{
		return (id >> detail::index_bits) & detail::generation_mask;
	}

	// Increment generation bit when we make a new object at the same ID
	constexpr id_type
	new_generation(id_type id)
	{
		const id_type generation{ id::generation(id) + 1 };
		assert(generation < (((u64)1 << detail::generation_bits) -1 ));
		return index(id) | (generation << detail::index_bits);
	}

	// Differentiates between debug build and release build to force id type
#if _DEBUG
	namespace detail {
		struct id_base
		{
			constexpr explicit id_base(id_type id) : _id{ id } {}
			constexpr operator id_type() const { return _id; }
		private:
			id_type _id;
		};
	}

#define DEFINE_TYPED_ID(name)							\
	struct name final : id::detail::id_base			\
	{													\
		constexpr explicit name(id::id_type id)			\
			: id_base{ id } {}							\
		constexpr name() : id_base{ 0 } {}				\
	};													\

#else
#define DEFINE_TYPED_ID(name) using name = id::id_type;
#endif

}