/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

#pragma once
#include "ComponentsCommon.h"


namespace savage::script {

	// Contains initialization information for script component
	struct init_info
	{
		detail::script_creator script_creator;
	};

	// Create script component
	component create(init_info info, game_entity::entity entity);
	// Remove script component
	void remove(component c);
	void update(float dt);
}