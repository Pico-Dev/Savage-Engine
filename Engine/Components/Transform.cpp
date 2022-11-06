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

#include "Transform.h"
#include "Entity.h"

namespace savage::transform
{
	namespace {

		utl::vector<math::v3> positions;
		utl::vector<math::v4> rotations;
		utl::vector<math::v3> scales;

	} // Anonymous namespace

	// Create transform component
	component create_transform(const init_info& info, game_entity::entity entity) 
	{
		assert(entity.is_valid()); // Must be valid entity
		const id::id_type entity_index{ id::index(entity.get_id()) };

		// If compoent we are trying to add is in the length of the aray of entities then we overwrite the open slot
		if (positions.size() > entity_index)
		{
			rotations[entity_index] = math::v4(info.rotation);
			positions[entity_index] = math::v3(info.position);
			scales[entity_index]	= math::v3(info.scale);
		}
		// If it is a new entity in the aray then add it to the end of the aray
		else
		{
			assert(positions.size() == entity_index);
			rotations.emplace_back(info.rotation);
			positions.emplace_back(info.position);
			scales.emplace_back(info.scale);
		}

		return component(transform_id{ (id::id_type)positions.size() - 1 });
	}
	// Remove transform component
	void remove_transform(component c)
	{
		assert(c.is_valid());
	}

	math::v4 component::rotation() const
	{
		assert(is_valid()); // Must be valid
		return rotations[id::index(_id)];
	}
	math::v3 component::position() const
	{
		assert(is_valid()); // Must be valid
		return positions[id::index(_id)];
	}
	math::v3 component::scale() const
	{
		assert(is_valid()); // Must be valid
		return scales[id::index(_id)];
	}
}