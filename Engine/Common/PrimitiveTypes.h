/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

#pragma once
#include <stdint.h>

// Unsigned integers

using u64 = uint64_t;
using u32 = uint32_t;
using u16 = uint16_t;
using u8  = uint8_t;

// Signed integers

using s64 = int64_t;
using s32 = int32_t;
using s16 = int16_t;
using s8  = int8_t;

// Set invalid value to -1
constexpr u64 u64_invalid_id { 0xffff'ffff'ffff'ffffui64 };
constexpr u32 u32_invalid_id { 0xffff'ffffui32 };
constexpr u16 u16_invalid_id { 0xffffui16 };
constexpr u8  u8_invalid_id  { 0xffui8 };

// Floats
using f32 = float;