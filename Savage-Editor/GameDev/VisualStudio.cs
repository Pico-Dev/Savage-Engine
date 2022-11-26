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

//using Microsoft.VisualStudio.OLE.Interop;
using Savage_Editor.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Runtime.InteropServices.ComTypes;
using System.Linq;
using System.IO;

namespace Savage_Editor.GameDev
{
	static class VisualStudio
	{
		// Reference to the projects VS instance
		private static EnvDTE80.DTE2 _vsInstance = null;
		// VS program ID
		private static readonly string _progID = "VisualStudio.DTE.17.0";

		[DllImport("ole32.dll")]
		private static extern int CreateBindCtx(uint reserverd, out IBindCtx ppbc);

		[DllImport("ole32.dll")]
		private static extern int GetRunningObjectTable(uint reserved, out IRunningObjectTable pprot);

		public static void OpenVisualStudio(string soulutionPath)
		{
			IRunningObjectTable rot = null;
			IEnumMoniker monikerTable = null;
			IBindCtx bindCtx = null;
			try
			{
				// See if the project already has a VS instance
				if (_vsInstance == null) // First try to find and open VS
				{
					var hResult = GetRunningObjectTable(0, out rot); // Get running object table and store if it found something
					if (hResult < 0 || rot == null) throw new COMException($"GetRunningObjectTable() returned HRESULT: {hResult:x8}"); // Test if nothing was found and throw error if needed

					// Get the table to be used for enumeration
					rot.EnumRunning(out monikerTable);
					monikerTable.Reset(); // Must start at first spot in list

					hResult = CreateBindCtx(0, out bindCtx); // Try to find a binding context and store if it was found
					if (hResult < 0 || bindCtx == null) throw new COMException($"CreateBindCtx() returned HRESULT: {hResult:x8}"); // Test if nothing was found and throw error if needed

					IMoniker[] currentMoniker = new IMoniker[1];
					while(monikerTable.Next(1, currentMoniker, IntPtr.Zero) == 0)
					{ 
						string name = string.Empty; // Process Name
						currentMoniker[0]?.GetDisplayName(bindCtx, null, out name); // Get display name of process in the bind context
						if (name.Contains(_progID)) // Check if VS instance was found
						{
							// Check if it is the right VS instance for the game project
							hResult = rot.GetObject(currentMoniker[0], out object obj);
							if (hResult < 0 || obj == null) throw new COMException($"Running Object Table's GetObject() returned HRESULT: {hResult:x8}"); // Test if nothing was found and throw error if needed

							EnvDTE80.DTE2 dte = obj as EnvDTE80.DTE2; // Cast obj to VS
							// Compare the name of the solution with the name of the VS instance
							var solutionName = dte.Solution.FullName;
							if(solutionName == soulutionPath)
							{
								_vsInstance = dte;
								break;
							}
						}
						
					}

					// If we could not find an instance
					if (_vsInstance == null)
					{
						// Create a new instance of VS
						Type visualStudioType = Type.GetTypeFromProgID(_progID, true);
						_vsInstance = Activator.CreateInstance(visualStudioType) as EnvDTE80.DTE2;
					}
				}
			}
			catch(Exception ex)
			{
				Debug.Write(ex.Message);
				Logger.Log(MessageType.Error, "Failed to open Visual Studio 2022");
			}
			// Release the COM objects
			finally
			{
				if (monikerTable != null) Marshal.ReleaseComObject(monikerTable);
				if (rot != null) Marshal.ReleaseComObject(rot);
				if (bindCtx != null) Marshal.ReleaseComObject(bindCtx);
			}
		}

		public static void CloseVisualStudio()
		{
			// Check for a valid solution then save and close
			if (_vsInstance?.Solution.IsOpen == true)
			{
				_vsInstance.ExecuteCommand("File.SaveAll");
				_vsInstance.Solution.Close(true);
			}
			_vsInstance?.Quit();
		}

		public static bool AddFilesToSolution(string solution, string projectName, string[] files)
		{
			Debug.Assert(files?.Length > 0);
			OpenVisualStudio(solution);
			try
			{
				if(_vsInstance != null)
				{
					// Open the solution if it is not already
					if (!_vsInstance.Solution.IsOpen) _vsInstance.Solution.Open(solution);
					// Save all files otherwise
					else _vsInstance.ExecuteCommand("File.SaveAll");

					// Go through all the projects in the solution and find the one with same name as the game project
					foreach (EnvDTE.Project project in _vsInstance.Solution.Projects)
					{
						// once a solution with the project name is found we add the files from the array
						if(project.UniqueName.Contains(projectName))
						{
							foreach (var file in files)
							{
								project.ProjectItems.AddFromFile(file);
							}
						}
					}

					// Open the file and show it
					var cpp = files.FirstOrDefault(x => Path.GetExtension(x) == ".cpp");
					if(!string.IsNullOrEmpty(cpp))
					{
						_vsInstance.ItemOperations.OpenFile(cpp, EnvDTE.Constants.vsViewKindAny).Visible = true;
					}
					_vsInstance.MainWindow.Activate();
					_vsInstance.MainWindow.Visible = true;
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
				Logger.Log(MessageType.Error, "Failed to add files to VS project");
				return false;
			}
			return true;
		}
	}
}
