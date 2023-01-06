/*
Copyright (c) 2022 Daniel McLarty
Copyright (c) 2020-2022 Arash Khatami

MIT License - see LICENSE file
*/

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

namespace Savage_Editor.Utilities
{
	enum MessageType
	{
		Info = 0x01,
		Warning = 0x02,
		Error = 0x04
	}

	class LogMessage
	{
		public DateTime Time { get; }
		public MessageType MessageType { get; }
		public string Message { get; }
		public string File { get; }
		public string Caller { get; }
		public int Line { get; }
		public string MetaData => $"{File}: {Caller} ({Line})";

		// Design a log message
		public LogMessage(MessageType type, string msg, string file, string caller, int line)
		{
			Time = DateTime.Now;
			MessageType = type;
			Message = msg;
			File = Path.GetFileName(file);
			Caller = caller;
			Line = line;
		}
	}

	static class Logger
	{
		private static int _messageFileter = (int)(MessageType.Info | MessageType.Warning | MessageType.Error); // OR the bits together for the type
		private static readonly ObservableCollection<LogMessage> _messages = new ObservableCollection<LogMessage>();
		public static ReadOnlyObservableCollection<LogMessage> Messages
		{ get; } = new ReadOnlyObservableCollection<LogMessage>(_messages);
		public static CollectionViewSource FilterdMessages
		{ get; } = new CollectionViewSource() { Source = Messages };

		public static async void Log(MessageType type, string msg, [CallerFilePath] string file = "", [CallerMemberName] string caller = "", [CallerLineNumber] int line = 0)
		{
			// Wait for UI thread
			await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
			{
				// Add log to log list
				_messages.Add(new LogMessage(type, msg, file, caller, line));
			}));
		}

		public static async void Clear()
		{
			// Wait for UI thread
			await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
			{
				// Clear log list
				_messages.Clear();
			}));
		}

		// Set message filter
		public static void SetMessageFilter(int mask)
		{
			_messageFileter = mask;
			FilterdMessages.View.Refresh();
		}

		static Logger()
		{
			// Called every time the filter refreshes
			FilterdMessages.Filter += (s, e) =>
			{
				// Get type of item to see if it should be in the filtered list
				var type = (int)(e.Item as LogMessage).MessageType;
				e.Accepted = (type & _messageFileter) != 0;
			};
		}
	}
}
