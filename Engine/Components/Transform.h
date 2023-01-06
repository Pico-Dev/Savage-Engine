/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

#pragma once
#include "ComponentsCommon.h"


namespace savage::transform {

	// Contains initialization information for transform component
	struct init_info
	{
		f32 position[3]{};
		f32 rotation[4]{};
		f32 scale[3]{1.f, 1.f, 1.f};
	};

	// Create transform component
	component create(init_info info, game_entity::entity entity);
	// Remove transform component
	void remove(component c);
}