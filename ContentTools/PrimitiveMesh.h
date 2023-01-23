/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

#pragma once
#include "ToolsCommon.h"

namespace savage::tools {
	// Define the types of procedural meshes 
	enum primitive_mesh_type : u32
	{
		plane,
		cube,
		uv_sphere,
		ico_shpere,
		cylender,
		capsule,
		marching_cube,

		count
	};

	// Define the initial values of a procedural mesh
	struct primitive_init_info
	{
		primitive_mesh_type type;
		u32					segments[3]{ 1,1,1, };
		math::v3			size{ 1, 1,1 };
		u32					lod{ 0 };
	};
}
