/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

#include "ContentLoader.h"
#include "..\Components\Entity.h"
#include "..\Components\Transform.h"
#include "..\Components\Script.h"

#if !defined(SHIPPING)

#include <fstream>
#include <filesystem>
#include <Windows.h>

namespace savage::content {
	namespace {
		// Define component types
		enum component_type
		{
			transform,
			script,

			count
		};

		// Hold the entities
		utl::vector<game_entity::entity> entities;
		// Hold component information
		transform::init_info transform_info{};
		script::init_info script_info{};

		// Define reading a transform from binary
		bool read_transform(const u8*& data, game_entity::entity_info& info)
		{
			using namespace DirectX;
			f32 rotation[3];

			assert(!info.transform); // Check if pointer is set

			// Get the transform position, rotation, and scale from the binary
			memcpy(&transform_info.position[0], data, sizeof(transform_info.position)); // Copy the data
			data += sizeof(transform_info.position); // Move the read pointer
			memcpy(&rotation[0], data, sizeof(rotation)); // Copy the data to the temp value to change it to a quat
			data += sizeof(rotation); // Move the read pointer
			memcpy(&transform_info.scale[0], data, sizeof(transform_info.scale)); // Copy the data
			data += sizeof(transform_info.scale); // Move the read pointer

			// Convert the rotation to quat
			XMFLOAT3A rot{ &rotation[0] };
			XMVECTOR quat{ XMQuaternionRotationRollPitchYawFromVector(XMLoadFloat3A(&rot)) };
			XMFLOAT4A rot_quart{};
			XMStoreFloat4A(&rot_quart, quat);
			memcpy(&transform_info.rotation[0], &rot_quart.x, sizeof(transform_info.rotation)); // Copy the data from the temp value

			// Set a pointer to the transform info 
			info.transform = &transform_info;

			return true;
		}

		// Define reading a script form binary
		bool read_script(const u8*& data, game_entity::entity_info& info)
		{
			assert(!info.script);
			const u32 name_length{ *data }; data += sizeof(u32); // Read how long the name of the scrip is 
			if (!name_length) return false; // Should not be zero
			// Script names should never be more than 255 characters if so something very wrong
			assert(name_length < 256);
			char script_name[256];
			memcpy(&script_name[0], data, name_length); // Copy the data
			data += name_length; // Move the read pointer
			// Make the name a zero-terminated c-string
			script_name[name_length] = 0;
			script_info.script_creator = script::detail::get_script_creator(script::detail::string_hash()(script_name)); // Initialize the script in the engine

			// Set a pointer to the script info
			info.script = &script_info;

			// True if not null
			return script_info.script_creator != nullptr;
		}

		// Returns a bool to show if the content loaded. Takes a pointer reference and an entity info.
		using component_reader = bool(*)(const u8*&, game_entity::entity_info&);
		// Array of script creators
		component_reader component_readers[]
		{
			read_transform,
			read_script,
		};
		static_assert(_countof(component_readers) == component_type::count); // Each component needs a reader

	} // Anonymous namespace

	bool load_game()
	{
		// Set working directory to the executable path
		wchar_t path[MAX_PATH]; // get the 260 Windows path max length
		const u32 length{ GetModuleFileName(0, &path[0], MAX_PATH) }; // Get the full path to the executable
		if (!length || GetLastError() == ERROR_INSUFFICIENT_BUFFER) return false; // Throw an error
		std::filesystem::path p{ path };
		SetCurrentDirectory(p.parent_path().wstring().c_str());

		// Read game.bin and create the entities
		std::ifstream game("game.bin", std::ios::in | std::ios::binary);
		utl::vector<u8> buffer(std::istreambuf_iterator<char>(game), {});
		assert(buffer.size()); // Should have a size
		const u8* at{ buffer.data() };
		constexpr u32 su32{ sizeof(u32) };
		const u32 num_entities{ *at }; at += su32; // read the number of entities
		if (!num_entities) return false;

		// Load the entities
		for (u32 entity_index{ 0 }; entity_index < num_entities; ++entity_index)
		{
			game_entity::entity_info info{}; // Define the entity info for each entity
			const u32 entity_types{ *at }; at += su32; // Read the entity type
			const u32 num_components{ *at }; at += su32; // read the number of components
			if (!num_components) return false;

			for (u32 component_index{ 0 }; component_index < num_components; ++component_index)
			{
				const u32 component_type{ *at }; at += su32;
				assert(component_type < component_type::count); // Needs to be in the right range
				if (!component_readers[component_type](at, info)) return false;
			}

			// Create the entity
			assert(info.transform); // Needs a transform
			game_entity::entity entity{ game_entity::create(info) }; // Try and create the entity
			if (!entity.is_valid()) return false; // Check if it has a valid ID
			entities.emplace_back(entity); // Put the entity in the array of entities
		}

		// Check if we read all the data in the buffer then return true
		assert(at == buffer.data() + buffer.size());
		return true;
	}

	void unload_game()
	{
		// Throw away all the entities
		for (auto entity : entities)
		{
			game_entity::remove(entity.get_id());
		}
	}
}

#endif // !defined(SHIPPING)