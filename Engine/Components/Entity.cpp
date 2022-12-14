/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

#include "Entity.h"
#include "Transform.h"
#include "Script.h"

namespace savage::game_entity {

	namespace {

		// Vector of transforms
		utl::vector<transform::component>	transforms;
		utl::vector<script::component>	scripts;

		// Get an array of generations
		utl::vector<id::generation_type>	generations;
		// Get free unused IDs
		utl::deque<entity_id>				free_ids;

	} // Anonymous namespace

	// Create game entity and get its index
	entity create(entity_info info) 
	{
		assert(info.transform); // All game entities must have a transform
		if (!info.transform) return entity{};
		
		entity_id id;

		// Only reuse IDs if free ids is greater than the min deleted elements
		if (free_ids.size() > id::min_deleted_elements)
		{
			id = free_ids.front(); // Find first free slot
			assert(!is_alive(id));
			free_ids.pop_front(); // Remove it form the free ids as it being used
			// Increase the generation of the slot
			id = entity_id{ id::new_generation(id) };
			++generations[id::index(id)];
		}
		// Otherwise create a new ID
		else
		{
			// Add the entity to the first unused slot
			id = entity_id{ (id::id_type)generations.size() };
			generations.push_back(0);

			// Resize components
			// NOTE: we don't use resize() in order to keep the number of memory allocations low
			transforms.emplace_back();
			scripts.emplace_back();
		}

		const entity new_entity{ id };
		const id::id_type index{ id::index(id) };

		// Create transform component
		assert(!transforms[index].is_valid());
		transforms[index] = transform::create(*info.transform, new_entity);
		if (!transforms[index].is_valid()) return {};

		// Create script component
		if (info.script && info.script->script_creator)
		{
			assert(!scripts[index].is_valid());
			scripts[index] = script::create(*info.script, new_entity);
			assert(scripts[index].is_valid());
		}

		return new_entity;
	}

	// Remove game entity
	void remove(entity_id id) 
	{
		const id::id_type index{ id::index(id) };
		assert(is_alive(id)); // Should be alive
		
		if (scripts[index].is_valid())
		{
			// Remove the script
			script::remove(scripts[index]);
			scripts[index] = {};
		}

		transform::remove(transforms[index]); // Remove the transform
		transforms[index] = {};
		free_ids.push_back(id); // Free the spot in the array
		
	}

	// Check if entity has same generation as spot
	bool is_alive(entity_id id) 
	{
		assert(id::is_valid(id)); // Must be valid
		const id::id_type index{ id::index(id) }; // Get entity index
		assert(index < generations.size()); // Index must be within current generations
		return (generations[index] == id::generation(id) && transforms[index].is_valid()); // Return if they are the same generation otherwise it is not "alive"
	}

	transform::component entity::transform() const
	{
		assert(is_alive(_id));
		const id::id_type index{ id::index(_id) }; // Get index
		return transforms[index]; // Return the transform for the index
	}

	script::component entity::script() const
	{
		assert(is_alive(_id));
		const id::id_type index{ id::index(_id) }; // Get index
		return scripts[index]; // Return the script for the index
	}

}