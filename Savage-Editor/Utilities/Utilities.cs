/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using System;
using System.Windows.Threading;

namespace Savage_Editor.Utilities
{
	public static class ID
	{
		public static int INVALID_ID => -1;
		public static bool IsValid(int id) => id != INVALID_ID;
	}

	public static class MathUtil
	{
		public static float Epsilon => 0.00001f;

		// Allow for close enough because floats are evil
		public static bool IsTheSameAs(this float value, float other)
		{
			return Math.Abs(value - other) < Epsilon;
		}

		// Allow for close enough with null-able values because floats are evil
		public static bool IsTheSameAs(this float? value, float? other)
		{
			if (!value.HasValue || !other.HasValue) return false;
			return Math.Abs(value.Value - other.Value) < Epsilon;
		}
	}

	class DelayEventTimerArgs : EventArgs
	{
		// Do we want to call the event
		public bool RepeatEvent { get; set; }
		public object Data { get; set; }

		public DelayEventTimerArgs(object data)
		{
			Data = data;
		}
	}

	// Stop events from happening too fast
	class DelayEventTimer
	{
		private readonly DispatcherTimer _timer;
		private readonly TimeSpan _delay;
		private DateTime _lastEventTime = DateTime.Now;
		private object _data;

		public event EventHandler<DelayEventTimerArgs> Triggerd;

		public void Trigger(object data = null)
		{
			_data = data;
			_lastEventTime = DateTime.Now;
			_timer.IsEnabled = true;
		}

		public void Disable()
		{
			_timer.IsEnabled = false;
		}

		private void OnTimerTick(object sender, EventArgs e)
		{
			if ((DateTime.Now - _lastEventTime) < _delay) return;
			var eventArgs = new DelayEventTimerArgs(_data);
			Triggerd?.Invoke(this, eventArgs);
			_timer.IsEnabled = eventArgs.RepeatEvent;
		}

		// Sets the delay before the same event can be called again
		public DelayEventTimer(TimeSpan delay, DispatcherPriority priority = DispatcherPriority.Normal)
		{
			_delay = delay;
			_timer = new DispatcherTimer(priority)
			{
				Interval = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 0.5)
			};
			_timer.Tick += OnTimerTick;
		}
	}
}
