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

#include "Common.h"
#include "CommonHeaders.h"
#include "ID.h"
#include "..\Engine\Components\Entity.h"
#include "..\Engine\Components\Transform.h"
#include "..\Engine\Components\script.h"

using namespace savage;

namespace {

	// Transform Description 
	struct transform_component
	{
		f32 position[3];
		f32 rotation[3];
		f32 scale[3];

		transform::init_info to_init_info()
		{
			using namespace DirectX;
			transform::init_info info{};
			memcpy(&info.position[0], &position[0], sizeof(position)); // Copy position values as is
			memcpy(&info.scale[0], &scale[0], sizeof(scale)); // Copy scale values as is
			XMFLOAT3A rot{ &rotation[0] }; // Gets the rotation form the editor
			// Transform Euler angle for rotation form editor to quaternion for the engine
			XMVECTOR quat{ XMQuaternionRotationRollPitchYawFromVector(XMLoadFloat3A(&rot)) };
			// Save it to an array for the engine
			XMFLOAT4A rot_quat{};
			XMStoreFloat4A(&rot_quat, quat);
			memcpy(&info.rotation[0], &rot_quat.x, sizeof(rotation)); // Return translated quaternion value to engine
			return info;
		}
	};

	// Script Description
	struct script_component
	{
		// Include information from the editor then pass to the create function
		script::detail::script_creator script_creator;

		script::init_info to_init_info()
		{
			script::init_info info{};
			info.script_creator = script_creator;
			return info;
		}
	};

	// List of components
	struct game_entity_descriptor
	{
		transform_component transform;
		script_component script;
	};

	game_entity::entity entity_from_id(id::id_type id)
	{
		return game_entity::entity{ game_entity::entity_id{id} };
	}
} // Anonymous namespace

EDITOR_INTERFACE id::id_type CreateGameEntity(game_entity_descriptor* e)
{
	assert(e);
	//Convert editor info to engine info
	game_entity_descriptor& desc{ *e };
	transform::init_info transform_info{ desc.transform.to_init_info() };
	script::init_info script_info{ desc.script.to_init_info() };
	game_entity::entity_info entity_info
	{
		&transform_info,
		&script_info,
	};
	return game_entity::create(entity_info).get_id();
}

EDITOR_INTERFACE void RemoveGameEntity(id::id_type id)
{
	assert(id::is_valid(id));
	game_entity::remove(game_entity::entity_id{ id });
}