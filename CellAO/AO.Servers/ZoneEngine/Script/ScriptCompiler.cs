﻿#region License
// Copyright (c) 2005-2012, CellAO Team
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

#region Usings...
#endregion

#region NameSpace
namespace ZoneEngine.Script
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using AO.Core;

    using Microsoft.CSharp;

    #region Class ScriptCompiler
    /// <summary>
    /// Controls Compilation and loading
    /// of *.cs files contained in the
    /// parent and subdirectories
    /// of the "Scripts\\" Directory.
    /// </summary>
    public class ScriptCompiler : IDisposable
    {
        #region Fields
        /// <summary>
        /// Our CSharp compiler object
        /// </summary>
        private readonly CodeDomProvider compiler =
            new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v4.0" } });

        // Holder for usermade scripts
        private readonly Dictionary<string, Type> scriptList = new Dictionary<string, Type>();

        // Holder for Chat commands
        private readonly Dictionary<string, Type> chatCommands = new Dictionary<string, Type>();

        /// <summary>
        /// Our compiler parameter command line to pass 
        /// when we compile the scripts.
        /// </summary>
        private readonly CompilerParameters p = new CompilerParameters
            {
                GenerateInMemory = false,
                GenerateExecutable = false,
                IncludeDebugInformation = true,
                OutputAssembly = "Scripts.dll",
                // TODO: Figure out how to parse the file and return the usings, then load those.
                ReferencedAssemblies =
                    {
                        "System.dll",
                        "System.Core.dll",
                        "AO.Core.dll",
                        "Cell.Core.dll",
                        "MySql.Data.dll",
                        "ZoneEngine.exe",
                        "ChatEngine.exe",
                        "LoginEngine.exe"
                    },
                TreatWarningsAsErrors = false,
                WarningLevel = 3,
                CompilerOptions = "/optimize"
            };
        #endregion

        #region Properties
        private string[] ScriptsList { get; set; }
        #endregion

        private readonly List<Assembly> multipleDllList = new List<Assembly>();

        #region Compiler
        /// <summary>
        /// 
        /// </summary>
        /// <param name="multipleFiles"></param>
        /// <returns></returns>
        public bool Compile(bool multipleFiles)
        {
            if (!this.LoadFiles())
            {
                return false;
            }
            if (multipleFiles)
            {
                LogScriptAction(
                    "ScriptCompiler:",
                    ConsoleColor.Yellow,
                    "multiple scripts configuration active.",
                    ConsoleColor.Magenta);
                foreach (string scriptFile in this.ScriptsList)
                {
                    this.p.OutputAssembly = String.Format(
                        CultureInfo.CurrentCulture, Path.Combine("tmp", DllName(scriptFile)));
                    // Create the directory if it doesnt exist
                    FileInfo file = new FileInfo(Path.Combine("tmp", DllName(scriptFile)));
                    if (file.Directory != null)
                    {
                        file.Directory.Create();
                    }
                    // Now compile the dll's
                    CompilerResults results = this.compiler.CompileAssemblyFromFile(this.p, scriptFile);
                    // And check for errors
                    if (ErrorReporting(results).Length != 0) // We have errors, display them
                    {
                        LogScriptAction("Error:", ConsoleColor.Yellow, ErrorReporting(results), ConsoleColor.Red);
                        return false;
                    }
                    LogScriptAction(
                        "Script " + scriptFile,
                        ConsoleColor.Green,
                        "Compiled to: " + this.p.OutputAssembly,
                        ConsoleColor.Green);
                    // Add the compiled assembly to our list
                    this.multipleDllList.Add(Assembly.LoadFile(file.FullName));
                }
                // Ok all good, load em
                foreach (Assembly a in this.multipleDllList)
                {
                    RunScript(a);
                }
            }
            else
            {
                // Compile the full Scripts.dll
                CompilerResults results = this.compiler.CompileAssemblyFromFile(this.p, this.ScriptsList);
                // And check for errors
                if (ErrorReporting(results).Length != 0) // We have errors, display them
                {
                    LogScriptAction("Error:", ConsoleColor.Yellow, ErrorReporting(results), ConsoleColor.Red);
                    return false;
                }
                //Load the full dll
                try
                {
                    FileInfo file = new FileInfo("Scripts.dll");
                    Assembly asm = Assembly.LoadFile(file.FullName);
                    this.multipleDllList.Add(asm);
                    RunScript(asm);
                }
                catch (FileLoadException ee)
                {
                    LogScriptAction(
                        "ERROR", ConsoleColor.Red, "File loading not successful:\r\n" + ee, ConsoleColor.Red);
                    return false;
                }
                catch (FileNotFoundException ee)
                {
                    LogScriptAction("ERROR", ConsoleColor.Red, "Script not found:\r\n" + ee, ConsoleColor.Red);
                    return false;
                }
                catch (BadImageFormatException ee)
                {
                    LogScriptAction("ERROR", ConsoleColor.Red, "Bad image format:\r\n" + ee, ConsoleColor.Red);
                    return false;
                }
                this.AddScriptMembers();
            }

            return true;
        }
        #endregion Compiler

        #region AppDomain Script Loading using interface IAOScript
        /// <summary>
        /// Loads all classes contained in our
        /// Assembly file that publically inherit
        /// our IAOScript class.
        /// Entry point for each script is public void Main(string[] args){}
        /// </summary>
        /// <param name="script">Our .NET dll or exe file.</param>
        private static void RunScript(Assembly script)
        {
            // Now that we have a compiled script, lets run them
            foreach (Type type in script.GetExportedTypes()) // returns all public types in the asembly
            {
                foreach (Type iface in type.GetInterfaces())
                {
                    if (iface == typeof(IAOScript))
                    {
                        // yay, we found a script interface, lets create it and run it!
                        // Get the constructor for the current type
                        // you can also specify what creation parameter types you want to pass to it,
                        // so you could possibly pass in data it might need, or a class that it can use to query the host application
                        ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                        if (constructor != null && constructor.IsPublic)
                        {
                            // lets be friendly and only do things legitimitely by only using valid constructors

                            // we specified that we wanted a constructor that doesn't take parameters, so don't pass parameters
                            IAOScript scriptObject = constructor.Invoke(null) as IAOScript;
                            if (scriptObject != null)
                            {
                                LogScriptAction(
                                    "Script",
                                    ConsoleColor.Green,
                                    scriptObject.GetType().Name + " Loaded.",
                                    ConsoleColor.Green);
                                //Lets run our script and display its results
                                scriptObject.Main(null);
                            }
                            else
                            {
                                // hmmm, for some reason it didn't create the object
                                // this shouldn't happen, as we have been doing checks all along, but we should
                                // inform the user something bad has happened, and possibly request them to send
                                // you the script so you can debug this problem
                                LogScriptAction("Error!", ConsoleColor.Red, "Script not loaded.", ConsoleColor.Red);
                            }
                        }
                        else
                        {
                            // and even more friendly and explain that there was no valid constructor
                            // found and thats why this script object wasn't run
                            LogScriptAction("Error!", ConsoleColor.Red, "No valid constructor found.", ConsoleColor.Red);
                        }
                    }
                }
            }
        }
        #endregion AppDomain Script Loading using interface IAOScript

        #region ErrorReporting Logging and Misc Tools
        /// <summary>
        /// Our Error reporting method.
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        private static string ErrorReporting(CompilerResults results)
        {
            StringBuilder report = new StringBuilder();
            if (results.Errors.HasErrors)
            {
                //Count the errors and return them

                var count = results.Errors.Count;
                for (var i = 0; i < count; i++)
                {
                    report.Append(results.Errors[i].FileName);
                    report.AppendLine(
                        " In Line: " + results.Errors[i].Line + " Error: " + results.Errors[i].ErrorNumber + " "
                        + results.Errors[i].ErrorText);
                }
            }

            return report.ToString();
        }

        /// <summary>
        /// Remove all text in a string before
        /// the first chars it finds.
        /// If chars is '\\' then 
        /// Debug\\Scripts turns into Scripts
        /// </summary>
        /// <param name="hayStack">The string to trim the front of.</param>
        /// <param name="needle">
        /// The first text char in the string 
        /// that matches this, and everything before it will be removed.
        /// </param>
        /// <returns>The corrected string.</returns>
        public static string RemoveCharactersBeforeChar(string hayStack, char needle)
        {
            string input = hayStack;
            int index = input.IndexOf(needle);
            if (index >= 0)
            {
                return input.Substring(index + 1);
            }
            //Hmm if we got here then it has no .'s in it so just return input
            return input;
        }

        /// <summary>
        /// Removes all text from a string
        /// after char chars
        /// </summary>
        /// <param name="hayStack">The string to trim.</param>
        /// <param name="needle">The char to remove all text after EX: '.'</param>
        /// <returns>The corrected string.</returns>
        public static string RemoveCharactersAfterChar(string hayStack, char needle)
        {
            string input = hayStack;
            int index = input.IndexOf(needle);
            if (index > 0)
            {
                input = input.Substring(0, index);
            }
            return input;
        }

        /// <summary>
        /// Turn our script names into dll names.
        /// </summary>
        /// <param name="scriptName">The script name.</param>
        /// <returns>The dll name.</returns>
        public static string DllName(string scriptName)
        {
            scriptName = RemoveCharactersAfterChar(scriptName, '.');
            scriptName = RemoveCharactersBeforeChar(scriptName, '\\');
            scriptName = RemoveCharactersBeforeChar(scriptName, '/');

            return scriptName + ".dll";
        }

        /// <summary>
        /// If the Scripts directory is empty
        /// or the Scripts directory is missing
        /// Give the correct error.
        /// </summary>
        /// <returns>true if the Scripts directory exsits, and there is at least one script in it.</returns>
        private bool LoadFiles()
        {
            // Seperated like this, because i want to display different custom errors.
            try
            {
                this.ScriptsList = Directory.GetFiles("Scripts", "*.cs", SearchOption.AllDirectories);
            }
            catch (DirectoryNotFoundException)
            {
                LogScriptAction("Error", ConsoleColor.Red, "Scripts directory does not exist!", ConsoleColor.Red);
                return false;
            }
            catch (PathTooLongException)
            {
                LogScriptAction("Error", ConsoleColor.Red, "Path name is too long", ConsoleColor.Red);
                return false;
            }
            catch (ArgumentException)
            {
                LogScriptAction("Error", ConsoleColor.Red, "Path is zero length or has invalid chars", ConsoleColor.Red);
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                LogScriptAction(
                    "Error", ConsoleColor.Red, "You don't have permission to access this directory", ConsoleColor.Red);
                return false;
            }
            catch (IOException)
            {
                LogScriptAction(
                    "Error",
                    ConsoleColor.Red,
                    "I/O Error occured. (Path is filename or network error)",
                    ConsoleColor.Red);
                return false;
            }

            if (this.ScriptsList.Length == 0)
            {
                LogScriptAction(
                    "Error:", ConsoleColor.Red, "Scripts directory contains no scripts!", ConsoleColor.Yellow);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Logs messages to the console.
        /// </summary>
        /// <param name="owner">Who or what to log as.</param>
        /// <param name="ownerColor">The color of the text the owner shows.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="messageColor">The color to display the message in.</param>
        public static void LogScriptAction(
            string owner, ConsoleColor ownerColor, string message, ConsoleColor messageColor)
        {
            Console.ForegroundColor = ownerColor;
            Console.Write(owner + " ");
            Console.ForegroundColor = messageColor;
            Console.Write(message + "\n");
            Console.ResetColor();
        }
        #endregion ErrorReporting Logging and Misc Tools

        #region Read all Classes and their Members into a Dictionary
        public void AddScriptMembers()
        {
            this.scriptList.Clear();
            foreach (Assembly assembly in this.multipleDllList)
            {
                foreach (Type t in assembly.GetTypes())
                {
                    if (t.GetInterface("IAOScript") != null)
                    {
                        if (t.Name != "IAOScript")
                        {
                            foreach (MemberInfo mi in t.GetMembers())
                            {
                                if ((mi.Name == "GetType") || (mi.Name == ".ctor") || (mi.Name == "GetHashCode")
                                    || (mi.Name == "ToString") || (mi.Name == "Equals"))
                                {
                                    continue;
                                }
                                if (mi.MemberType == MemberTypes.Method)
                                {
                                    if (!this.scriptList.ContainsKey(t.Namespace + "." + t.Name + ":" + mi.Name))
                                    {
                                        this.scriptList.Add(t.Namespace + "." + t.Name + ":" + mi.Name, t);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            this.chatCommands.Clear();
            Assembly wholeassembly = Assembly.GetExecutingAssembly();
            foreach (Type t in wholeassembly.GetTypes())
            {
                if (t.Namespace == "ZoneEngine.ChatCommands")
                {
                    if (t.Name != "AOChatCommand")
                    {
                        if (!this.chatCommands.ContainsKey(t.Namespace + "." + t.Name))
                        {
                            AOChatCommand aoc = (AOChatCommand)wholeassembly.CreateInstance(t.Namespace + "." + t.Name);
                            List<string> acceptedcommands = aoc.ListCommands();
                            foreach (string command in acceptedcommands)
                            {
                                this.chatCommands.Add(t.Namespace + "." + t.Name + ":" + command, t);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Function Calling
        public void CallMethod(string functionName, Character character)
        {
            foreach (Assembly assembly in this.multipleDllList)
            {
                foreach (KeyValuePair<string, Type> kv in this.scriptList)
                {
                    if (kv.Key.Substring(kv.Key.IndexOf(":", StringComparison.Ordinal)) == ":" + functionName)
                    {
                        IAOScript aoScript =
                            (IAOScript)
                            assembly.CreateInstance(kv.Key.Substring(0, kv.Key.IndexOf(":", StringComparison.Ordinal)));
                        if (aoScript != null)
                        {
                            kv.Value.InvokeMember(
                                functionName,
                                BindingFlags.Default | BindingFlags.InvokeMethod,
                                null,
                                aoScript,
                                new object[] { character },
                                CultureInfo.InvariantCulture);
                        }
                    }
                }
            }
        }
        #endregion

        #region ChatCommand Calling
        public void CallChatCommand(string commandName, Client client, Identity target, string[] commandArguments)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            if (commandName.ToUpperInvariant() != "LISTCOMMANDS")
            {
                foreach (KeyValuePair<string, Type> kv in this.chatCommands)
                {
                    if (kv.Key.Substring(kv.Key.IndexOf(":", StringComparison.Ordinal) + 1).ToUpperInvariant()
                        == commandName.ToUpperInvariant())
                    {
                        AOChatCommand aoc =
                            (AOChatCommand)
                            assembly.CreateInstance(kv.Key.Substring(0, kv.Key.IndexOf(":", StringComparison.Ordinal)));
                        if (aoc != null)
                        {
                            // Check GM Level bitwise
                            if ((client.Character.Stats.GMLevel.Value < aoc.GMLevelNeeded())
                                && (aoc.GMLevelNeeded() > 0))
                            {
                                client.SendChatText(
                                    "You are not authorized to use this command!. This incident will be recorded.");
                                // It is not yet :)
                                return;
                            }
                            // Check if only one argument has been passed for "help"
                            if (commandArguments.Length == 2)
                            {
                                if (commandArguments[1].ToUpperInvariant() == "HELP")
                                {
                                    aoc.CommandHelp(client);
                                    return;
                                }
                            }
                            // Execute the command with the given command arguments, if CheckCommandArguments is true else print command help
                            if (aoc.CheckCommandArguments(commandArguments))
                            {
                                aoc.ExecuteCommand(client, target, commandArguments);
                            }
                            else
                            {
                                aoc.CommandHelp(client);
                            }
                        }
                    }
                }
            }
            else
            {
                client.SendChatText("Available Commands:");
                string[] scriptNames = this.chatCommands.Keys.ToArray();
                for (int i = 0; i < scriptNames.Length; i++)
                {
                    scriptNames[i] = scriptNames[i].Substring(scriptNames[i].IndexOf(":", StringComparison.Ordinal) + 1)
                                     + ":"
                                     +
                                     scriptNames[i].Substring(0, scriptNames[i].IndexOf(":", StringComparison.Ordinal));
                }
                Array.Sort(scriptNames);

                foreach (string scriptName in scriptNames)
                {
                    string typename = scriptName.Substring(scriptName.IndexOf(":", StringComparison.Ordinal) + 1);
                    AOChatCommand aoc = (AOChatCommand)assembly.CreateInstance(typename);
                    if (aoc != null)
                    {
                        if (client.Character.Stats.GMLevel.Value >= aoc.GMLevelNeeded())
                        {
                            client.SendChatText(
                                scriptName.Substring(0, scriptName.IndexOf(":", StringComparison.Ordinal)));
                        }
                    }
                }
            }
        }
        #endregion ChatCommand Calling

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.compiler.Dispose();
            }
        }
    }
    #endregion Class ScriptCompiler
}

#endregion NameSpace