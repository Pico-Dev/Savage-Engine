/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

#pragma once
#include "..\Components\ComponentsCommon.h"

namespace savage::script {

	// Define typed ID for transform component
	DEFINE_TYPED_ID(script_id);

	class component final
	{
	public:
		// When making an instance of this class without a parameter for the constructor it will return an invalid ID
		constexpr explicit component(script_id id) : _id{ id } {}
		constexpr component() : _id{ id::invalid_id } {}
		// If the transform has a valid ID it will return that
		constexpr script_id get_id() const { return _id; }
		// Check if ID is valid
		constexpr bool is_valid() const { return id::is_valid(_id); }

	private:
		script_id _id;
	};
}