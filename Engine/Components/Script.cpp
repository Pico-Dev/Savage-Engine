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

#include "Script.h"
#include "Entity.h"

namespace savage::script
{
	namespace {

		utl::vector<detail::script_ptr>		entity_scripts;
		utl::vector<id::id_type>			id_mapping;

		utl::vector<id::generation_type>	generations;
		utl::deque<script_id>				free_ids;

		using script_registry = std::unordered_map<size_t, detail::script_creator>;

		script_registry& registry() 
		{
			// NOTE: This variable must be in the function because of the 
			//		 initialization order of the data. This way we can make sure
			//		 the data is initialized before using it.
			static script_registry reg;
			return reg;
		}

#ifdef USE_WITH_EDITOR
		utl::vector<std::string>& script_names()
		{
			// NOTE: This variable must be in the function because of the 
			//		 initialization order of the data. This way we can make sure
			//		 the data is initialized before using it.
			static utl::vector<std::string> names;
			return names;
		}
#endif // USE_WITH_EDITOR


		bool exists(script_id id)
		{
			assert(id::is_valid(id)); // ID must be valid
			const id::id_type index{ id::index(id) }; // Get ID index
			assert(index < generations.size() && id_mapping[index] < entity_scripts.size());
			assert(generations[index] == id::generation(id));
			// Return if it is the same generation and the index of the script is not null
			return (generations[index] == id::generation(id)) && entity_scripts[id_mapping[index]] && entity_scripts[id_mapping[index]] -> is_valid();
		}
	} // anonymous namespace

	namespace detail {
		// Register a script with the engine
		u8 register_script(size_t tag, script_creator func)
		{
			// Get the registry then add a pair with a tag and function pointer
			// Then returns a pair in which the second member is a bool that lets us know if the insert succeeded
			bool result{ registry().insert(script_registry::value_type{tag, func}).second };
			assert(result);
			return result;
		}

		script_creator get_script_creator(size_t tag)
		{
			// Lookup the script creator using its tag
			auto script = savage::script::registry().find(tag);
			assert(script != savage::script::registry().end() && script->first == tag);
			return script->second;
		}
		
#ifdef USE_WITH_EDITOR
		u8 add_script_name(const char* name)
		{
			script_names().emplace_back(name);
			return true;
		}
#endif // USE_WITH_EDITOR
	} // detail namespace

	component create(init_info info, game_entity::entity entity)
	{
		assert(entity.is_valid());
		assert(info.script_creator);

		script_id id{};
		// Only reuse IDs if free ids is greater than the min deleted elements
		if (free_ids.size() > id::min_deleted_elements)
		{
			id = free_ids.front(); // Find first free slot
			assert(!exists(id)); //Assert that id does not exist
			free_ids.pop_front(); // Remove it form the free ids as it being used
			// Increase the generation of the slot
			id = script_id{ id::new_generation(id) }; 
			++generations[id::index(id)];
		}
		// Otherwise create a new ID
		else
		{
			// Add the entity to the first unused slot
			id = script_id{ (id::id_type)id_mapping.size() };
			id_mapping.emplace_back();
			generations.push_back(0);
		}

		assert(id::is_valid(id));
		const id::id_type index{ (id::id_type)entity_scripts.size() };
		entity_scripts.emplace_back(info.script_creator(entity)); // Add instance to end of entity scripts
		assert(entity_scripts.back()->get_id() == entity.get_id()); // Id of script class and entity should be the same
		// Get location of where the entity script was added
		id_mapping[id::index(id)] = index;

		return component{ id };
	}

	void remove(component c)
	{
		assert(c.is_valid() && exists(c.get_id())); // Can't remove a dead object
		const script_id id{ c.get_id() };
		const id::id_type index{ id_mapping[id::index(id)] };
		const script_id last_id{ entity_scripts.back()->script().get_id() }; // Get the id of the script at the end of the list
		utl::erase_unordered(entity_scripts, index); // Remove the object in question
		id_mapping[id::index(last_id)] = index; // Reference the moved object to its old ID
		id_mapping[id::index(id)] = id::invalid_id; // Set the removed component to an invalid ID
	}

	void update(float dt)
	{
		// Goes through all scripts and calls the update function
		for (auto& ptr : entity_scripts)
		{
			ptr->update(dt);
		}
	}
}

#ifdef USE_WITH_EDITOR
#include <atlsafe.h>

extern "C" __declspec(dllexport)
LPSAFEARRAY
get_script_names()
{
	// Get the size of the strings
	const u32 size{ (u32)savage::script::script_names().size() };
	if (!size) return nullptr;
	// Reserve memory for the strings
	CComSafeArray<BSTR> names(size);
	for (u32 i{ 0 }; i < size; i++)
	{
		// Convert the strings to be used in the editor
		names.SetAt(i, A2BSTR_EX(savage::script::script_names()[i].c_str()), false);
	}
	// Returns array of names
	return names.Detach();
}
#endif // USE_WITH_EDITOR