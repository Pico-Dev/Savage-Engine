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

#include "Test.h"
#include "..\Engine\Components\Entity.h"
#include "..\Engine\Components\Transform.h"

#include <iostream>
#include <ctime>


using namespace savage;

class engine_test : public test
{
public:
	bool initialize() override 
	{
		srand((u32)time(nullptr)); // get random seed for testing
		return true; 
	}
	void run() override 
	{ 
		do {
			for (u32 i{ 0 }; i < 10000; i++) // Create and remove in batches of 10,000 to test for ID wrap around error
			{
				create_random();
				remove_random();
				_num_entities = (u32)_entities.size();
			}
			print_results();
		} while (getchar() != 'q'); // Test until 'q' is pressed
	}
	void shutdown() override 
	{ }

private:

	void create_random()
	{
		u32 count = rand() % 20; // Get random amount
		if (_entities.empty()) count = 1000; // keep a minimum of 1000 entities
		transform::init_info transform_info{}; // Create a transform component
		game_entity::entity_info entity_info{
			&transform_info // Add the transform info to each entity
		};

		while (count > 0)
		{
			++_added; // keep count
			game_entity::entity entity{ game_entity::create(entity_info) }; // Add the game entity
			assert(entity.is_valid() && id::is_valid(entity.get_id()));
			_entities.push_back(entity); // Record keeping
			assert(game_entity::is_alive(entity.get_id())); // It should be alive
			--count;
		}
	}

	void remove_random()
	{
		u32 count = rand() % 20; // Get random amount
		if (_entities.size() < 1000) return; // keep a minimum of 1000 entities
		while (count > 0)
		{
			const u32 index{ (u32)rand() % (u32)_entities.size() }; // Get where to remove an entity
			const game_entity::entity entity{ _entities[index] };
			assert(entity.is_valid() && id::is_valid(entity.get_id()));
			if (entity.is_valid())
			{
				game_entity::remove(entity.get_id()); // Remove game entities
				_entities.erase(_entities.begin() + index);
				assert(!game_entity::is_alive(entity.get_id())); // It should be dead
				++_removed; // Record keeping
			}
			--count;
		}
	}

	void print_results()
	{
		std::cout << "Entities Created: " << _added << std::endl;
		std::cout << "Entities Deleted: " << _removed << std::endl;
	}

	utl::vector<game_entity::entity> _entities;

	u32 _added{ 0 };
	u32 _removed{ 0 };
	u32 _num_entities{ 0 };
};
