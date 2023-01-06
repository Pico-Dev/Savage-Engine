/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using Savage_Editor.DLLWrappers;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace Savage_Editor.Utilities
{
	internal class RenderSurfaceHost : HwndHost
	{
		private readonly int _width = 800;
		private readonly int _height = 600;

		// Hold the handle to the render window
		private IntPtr _renderWindowHandle = IntPtr.Zero;

		// Hold the delay between calling resize events
		private DelayEventTimer _resizeTimer;

		// Hold the ID of the render surface
		public int SurfaceID { get; private set; } = ID.INVALID_ID;

		// Resize the render surface
		public void Resize()
		{
			_resizeTimer.Trigger();
		}

		private void Resize(object sender, DelayEventTimerArgs e)
		{
			// Don't happen when still resizing
			e.RepeatEvent = Mouse.LeftButton == MouseButtonState.Pressed;
			if (!e.RepeatEvent)
			{
				EngineAPI.ResizeRenderSurface(SurfaceID);
			}
		}

		// Set the with and height
		public RenderSurfaceHost(double width, double height)
		{
			_width = (int)width;
			_height = (int)height;
			_resizeTimer = new DelayEventTimer(TimeSpan.FromMilliseconds(250.0));
			_resizeTimer.Triggerd += Resize;
		}

		// Called when created
		protected override HandleRef BuildWindowCore(HandleRef hwndParent)
		{
			// Create a surface and get the ID
			SurfaceID = EngineAPI.CreateRenderSurface(hwndParent.Handle, _width, _height);
			Debug.Assert(ID.IsValid(SurfaceID));
			_renderWindowHandle = EngineAPI.GetWindowHandle(SurfaceID); // Get handle to the window
			Debug.Assert(_renderWindowHandle != IntPtr.Zero);

			return new HandleRef(this, _renderWindowHandle); // Create an instance of handle reference
		}

		// Called when closed
		protected override void DestroyWindowCore(HandleRef hwnd)
		{
			EngineAPI.RemoveRenderSurface(SurfaceID);
			SurfaceID = ID.INVALID_ID;
			_renderWindowHandle = IntPtr.Zero;
		}
	}
}
