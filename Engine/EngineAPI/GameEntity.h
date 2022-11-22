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

#pragma once

#include "..\Components\ComponentsCommon.h"
#include "TransformComponent.h"
#include "ScriptComponent.h"

namespace savage {
	namespace game_entity {

		DEFINE_TYPED_ID(entity_id);

		class entity {
		public:
			// When making an instance of this class without a parameter for the constructor it will return an invalid ID
			constexpr explicit entity(entity_id id) : _id{ id } {}
			constexpr entity() : _id{ id::invalid_id } {}
			// If the entity has a valid ID it will return that
			constexpr entity_id get_id() const { return _id; }
			// Check if ID is valid
			constexpr bool is_valid() const { return id::is_valid(_id); }

			// Get the transform
			transform::component transform() const;
			// Get the script
			script::component script() const;
		private:
			entity_id _id;
		};
	} // namespace game_entity
	namespace script
	{
		class entity_script : public game_entity::entity
		{
		public:
			virtual ~entity_script() = default;
			virtual void begin_play() {} // Called on the frame the entity is created
			virtual void update(float) {} // Called every frame the entity exists and takes the seconds per frame as an input
		protected:
			constexpr explicit entity_script(game_entity::entity entity) : game_entity::entity{ entity.get_id()} {}
		};

		// Don't want to expose explicitly to game code
		namespace detail {
			// Define script_ptr and script_creator and string_hash
			using script_ptr = std::unique_ptr<entity_script>;
			using script_creator = script_ptr(*)(game_entity::entity entity);
			using string_hash = std::hash<std::string>;

			// Register a script with the engine
			u8 register_script(size_t, script_creator);

			// Script creation function
			template<class script_class>
			script_ptr create_script(game_entity::entity entity)
			{
				assert(entity.is_valid());
				// Create an instance of the script and return a pointer to the script
				return std::make_unique<script_class>(entity);
			}
#define REGISTER_SCRIPT(TYPE)													\
			class TYPE;															\
			namespace {															\
				const u8 _reg_##TYPE{ savage::script::detail::register_script(	\
				savage::script::detail::string_hash()(#TYPE),					\
				&savage::script::detail::create_script<TYPE>) };				\
			}
		} // namespace detail
	} // namespace script
}