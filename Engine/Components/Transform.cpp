/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
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
	component create(init_info info, game_entity::entity entity) 
	{
		assert(entity.is_valid()); // Must be valid entity
		const id::id_type entity_index{ id::index(entity.get_id()) };

		// If component we are trying to add is in the length of the array of entities then we overwrite the open slot
		if (positions.size() > entity_index)
		{
			rotations[entity_index] = math::v4(info.rotation);
			positions[entity_index] = math::v3(info.position);
			scales[entity_index]	= math::v3(info.scale);
		}
		// If it is a new entity in the array then add it to the end of the array
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
	void remove(component c)
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