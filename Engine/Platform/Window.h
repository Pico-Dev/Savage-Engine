/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

#pragma once
#include "CommonHeaders.h"

namespace savage::platform 
{

	DEFINE_TYPED_ID(window_id)

	class Window
	{
	public:
		// When making an instance of this class without a parameter for the constructor it will return an invalid ID
		constexpr explicit Window(window_id id) : _id{ id } {}
		constexpr Window() : _id{ id::invalid_id } {}
		// If the transform has a valid ID it will return that
		constexpr window_id get_id() const { return _id; }
		// Check if ID is valid
		constexpr bool is_valid() const { return id::is_valid(_id); }

		// Window value definitions
		void set_fullscreen(bool is_fullscreen) const;
		bool is_fullscreen() const;
		void* handle() const;
		void set_caption(const wchar_t* caption) const;
		math::u32v4 size() const;
		void resize(u32 width, u32 height) const;
		u32 width() const;
		u32 height() const;
		bool is_closed() const;

	private:
		window_id _id{ id::invalid_id };
	};
}
