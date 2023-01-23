/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

#include "PrimitiveMesh.h"
#include "Geometry.h"

namespace savage::tools {
	namespace {

		// All primitive meshes creators need an init info reference and a scene reference
		using primitive_mesh_creator = void(*)(scene&, const primitive_init_info& info);

		void create_plane			(scene& scene, const primitive_init_info& info);
		void create_cube			(scene& scene, const primitive_init_info& info);
		void create_uv_sphere		(scene& scene, const primitive_init_info& info);
		void create_ico_shpere		(scene& scene, const primitive_init_info& info);
		void create_cylender		(scene& scene, const primitive_init_info& info);
		void create_capsule			(scene& scene, const primitive_init_info& info);
		void create_marching_cube	(scene& scene, const primitive_init_info& info);

		// Array of function pointers to creation functions
		primitive_mesh_creator creators[primitive_mesh_type::count]
		{
			create_plane,
			create_cube,
			create_uv_sphere,
			create_ico_shpere,
			create_cylender,
			create_capsule,
			create_marching_cube
		};

		static_assert(_countof(creators) == primitive_mesh_type::count);

	} // Anonymous namespace

	// DLL function to create a primitive mesh
	EDITOR_INTERFACE void CreatePrimitiveMesh(scene_data* data, primitive_init_info* info)
	{
		assert(data && info);
		assert(info->type < primitive_mesh_type::count);
		scene scene{};
	}
}