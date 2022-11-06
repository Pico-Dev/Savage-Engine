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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using static System.Net.WebRequestMethods;
using System.Windows.Interop;
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

		// Degine a log message
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
		{ get;  } = new ReadOnlyObservableCollection<LogMessage>(_messages);
		public static CollectionViewSource FilterdMessages
		{ get; } = new CollectionViewSource() { Source = Messages };

		public static async void Log(MessageType type, string msg, [CallerFilePath]string file="", [CallerMemberName]string caller="", [CallerLineNumber]int line=0)
		{
			// Wait for UI thread
			await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
			{
				// Add log to log list
				_messages.Add (new LogMessage(type, msg, file, caller, line));
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
			// Called every time the filter refeshes
			FilterdMessages.Filter += (s, e) =>
			{
				// Get type of item to see if it should be in the filterd list
				var type = (int)(e.Item as LogMessage).MessageType;
				e.Accepted = (type & _messageFileter) != 0;
			};
		}
	}
}
