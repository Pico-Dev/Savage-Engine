/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

#pragma once

// Set use of custom versions of STL functions
#define USE_STL_VECTOR 1
#define USE_STL_DEQUE 1

#if USE_STL_VECTOR
#include <vector>
namespace savage::utl {
	template<typename T>
	using vector = std::vector<T>;

	// Swap two elements in a vector and remove the last element in the vector
	template<typename T>
	void erase_unordered(std::vector<T>& v, size_t index)
	{
		// If the vector has two or more elements
		if (v.size() > 1)
		{
			// We will swap the element at the index with the last element
			std::iter_swap(v.begin() + index, v.end() - 1);
			// Then we will remove the last element
			v.pop_back();
		}
		// If the vector has one element or is empty
		else
		{
			// We will just clear the vector
			v.clear();
		}
	}
}
#endif

#if USE_STL_DEQUE
#include <deque>
namespace savage::utl {
	template<typename T>
	using deque = std::deque<T>;
}
#endif

namespace savage::util {

	// TODO: make custom version of STL functions

}