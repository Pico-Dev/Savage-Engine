/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

#pragma once
#include "ToolsCommon.h"

namespace savage::tools {

	// Defines a mesh for geometry as scratch space
	struct mesh 
	{
		// Initial data
		utl::vector<math::v3>				positions;
		utl::vector<math::v3>				normals;
		utl::vector<math::v3>				tangents;
		utl::vector<utl::vector<math::v2>>	uv_sets;

		utl::vector<u32>					raw_indices;

		// Intermediate data

		// Output data

	};

	// Defines LOD groups for the geometry
	struct lod_group
	{
		std::string			name;
		utl::vector<mesh>	meshes;
	};

	// Define a scene for the geometry
	struct scene
	{
		std::string				name;
		utl::vector<lod_group>	lod_groups;
	};

	// Define the import settings for geometry
	struct geometry_import_settings
	{
		f32 smothing_angle;
		u8  calculate_normals;
		u8  calculate_tangents;
		u8  reverse_handedness;
		u8  import_embeded_textures;
		u8  import_animations;
	};

	// Define the scene data for geometry 
	struct scene_data
	{
		u8*							buffer;
		u32							buffer_size;
		geometry_import_settings	settings;
	};

}