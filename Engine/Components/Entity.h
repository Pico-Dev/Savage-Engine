/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

#pragma once
#include "ComponentsCommon.h"

namespace savage {
	
// macro to define initialization information for components
#define INIT_INFO(component) namespace component { struct init_info; }

	INIT_INFO(transform);
	INIT_INFO(script);

#undef INIT_INFO

	namespace game_entity {
		// Initialization information
		struct entity_info
		{
			transform::init_info* transform{ nullptr };
			script::init_info* script{ nullptr };
		};

		// Create game entity and get its index
		entity create(entity_info info);
		// Remove game entity
		void remove(entity_id id);
		// Check if entity has same generation as spot
		bool is_alive(entity_id id);
	}
}