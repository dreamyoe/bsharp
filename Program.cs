using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.Threading;

namespace BSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args[0] == "help")
            {
                BSharp.ShowHelp();
            }

            if (args[0] == "version" || args[0] == "ver")
            {
                BSharp.ShowVersion();
            }

            BSharp.ExecuteCode(args[0]);
        }
    }

    public static class BSharp
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static Dictionary<string, string> assignedVariables = new Dictionary<string, string>();

        public static Dictionary<string, object> allowedGets = new Dictionary<string, object>();

        public static List<object> codeTypes = new List<object>
        {
            new CodeType("exit", "Exits the enviroment", "exit", () =>
            {
                Environment.Exit(0);
            }),
            new CodeType<string, string>("readline", "Reads input and puts it in an already declared variable", "readline <variable> <text>", (x, y) =>
            {
                if (string.IsNullOrEmpty(y))
                {
                    assignedVariables[x] = Console.ReadLine();
                }

                if (!string.IsNullOrEmpty(y))
                {
                    Console.WriteLine(y.Replace('|', ' '));
                    assignedVariables[x] = Console.ReadLine();
                }
            }),
            new CodeType<string>("wait", "Waits the specified time in miliseconds", "wait <ms>", (x) =>
            {
                Thread.Sleep(int.Parse(x));
            }),
            new CodeType("hide", "Hides the console window.", "hide", () =>
            {
                var handle = GetConsoleWindow();

                ShowWindow(handle, SW_HIDE);
            }),
            new CodeType("show", "Shows the console window.", "show", () =>
            {
                var handle = GetConsoleWindow();

                ShowWindow(handle, SW_SHOW);
            }),
            new CodeType<string>("print", "Prints text on the screen (Use | for a space)", "print <text>", (x) =>
            {
                if (x.StartsWith("$"))
                {
                    string key = x.Replace("$", string.Empty);
                    Console.WriteLine(assignedVariables[key]);
                    return;
                }

                string text = x.Replace('|', ' ');
                Console.WriteLine(text);
            }),
            new CodeType("pause", "Pauses and waits for next key input", "pause", () =>
            {
                Console.ReadKey();
            }),
            new CodeType<string, string>("add", "Adds two numbers up and prints the output", "add <1> <2>", (x, y) =>
            {
                Console.WriteLine(int.Parse(x) + int.Parse(y));
            }),
            new CodeType("clear", "Clears the screen.", "clear", () =>
            {
                Console.Clear();
            }),
            new CodeType<string, string>("var", "Makes a new variable for later use.", "var <name> <value>", (x, y) =>
            {
                assignedVariables.Add(x, y);
            }),
            new CodeType<string, string>("get", "Gets a specified value and puts the output into an declared variable", "get <type> <variable>", (x, y) => 
            {
                allowedGets.Clear();
                allowedGets.Add("username", Environment.UserName);
                allowedGets.Add("machinename", Environment.MachineName);
                allowedGets.Add("userdomainname", Environment.UserDomainName);
                allowedGets.Add("osversion", Environment.OSVersion);
                allowedGets.Add("systemdirectory", Environment.SystemDirectory);

                if (allowedGets[x] == null)
                {
                    Console.WriteLine($"There was an error running your code. Please check your code before running it again. Error: Invalid type {x} at get {x} {y}.");
                    Console.ReadKey();
                    Environment.Exit(0);
                    return;
                }

                assignedVariables[y] = (string) allowedGets[x];

                //Base src = (object)Environment;
                //assignedVariables[y] = src.GetType().GetProperty(x).GetValue(src, null).ToString();
            }),
        };

        public static void ShowVersion()
        {
            Console.WriteLine("BSharp v1.0.0.0\n(C) BSharp.\nAll rights reserved.");
            Environment.Exit(0);
        }

        public static void ShowHelp()
        {
            Console.WriteLine("\nBSharp HELP\n\nFormatting:\nRunning a file - BSharp <file location> (BSharp mycodefile.bs)\nDetermeting version - BSharp ver(sion)\n\nCode formatting:\n");
            for (int i = 0; i < codeTypes.Count; i++)
            {
                CodeTypeBase type = codeTypes[i] as CodeTypeBase;
                Console.WriteLine(type.codeFormat + " - " + type.codeDescription);
            }

            Environment.Exit(0);
        }

        static int i;

        public static void ExecuteCode(string fileName)
        {
            try
            {
                string[] lines = File.ReadAllLines(fileName);
                for (i = 0; i < lines.Length; i++)
                {
                    HandleCode(lines[i]);
                }
            }
            
            catch(Exception ex)
            {
                Console.WriteLine($"There where errors trying to run your code at line {i}. Please fix them in order for you code to run properly:\n" + ex.ToString());
            }
        }

        public static void HandleCode(string line)
        {
            string[] properties = line.Split(' ');

            for (int i = 0; i < codeTypes.Count; i++)
            {
                CodeTypeBase codeBase = codeTypes[i] as CodeTypeBase;

                if (line.Contains(codeBase.codeId))
                {
                    if (codeTypes[i] as CodeType != null)
                    {
                        (codeTypes[i] as CodeType).Invoke();
                    }

                    else if (codeTypes[i] as CodeType<string> != null)
                    {
                        (codeTypes[i] as CodeType<string>).Invoke(properties[1]);
                    }

                    else if (codeTypes[i] as CodeType<string, string> != null)
                    {
                        (codeTypes[i] as CodeType<string, string>).Invoke(properties[1], properties[2]);
                    }
                }
            }
        }
    }

    public class CodeTypeBase
    {
        private string _codeId;
        private string _codeDescription;
        private string _codeFormat;

        public string codeId { get { return _codeId; } }
        public string codeDescription { get { return _codeDescription; } }
        public string codeFormat { get { return _codeFormat; } }

        public CodeTypeBase(string id, string description, string format)
        {
            _codeId = id;
            _codeDescription = description;
            _codeFormat = format;
        }
    }

    public class CodeType : CodeTypeBase
    {
        private Action code;

        public CodeType(string id, string description, string format, Action code) : base(id, description, format)
        {
            this.code = code;
        }

        public void Invoke()
        {
            code.Invoke();
        }
    }

    public class CodeType<T1> : CodeTypeBase
    {
        private Action<T1> code;

        public CodeType(string id, string description, string format, Action<T1> code) : base(id, description, format)
        {
            this.code = code;
        }

        public void Invoke(T1 value)
        {
            code.Invoke(value);
        }
    }

    public class CodeType<T1, T2> : CodeTypeBase
    {
        private Action<T1, T2> code;

        public CodeType(string id, string description, string format, Action<T1, T2> code) : base(id, description, format)
        {
            this.code = code;
        }

        public void Invoke(T1 value, T2 value1)
        {
            code.Invoke(value, value1);
        }
    }

    public class CodeType<T1, T2, T3> : CodeTypeBase
    {
        private Action<T1, T2, T3> code;

        public CodeType(string id, string description, string format, Action<T1, T2, T3> code) : base(id, description, format)
        {
            this.code = code;
        }

        public void Invoke(T1 value, T2 value1, T3 value2)
        {
            code.Invoke(value, value1, value2);
        }
    }

    public class CodeType<T1, T2, T3, T4> : CodeTypeBase
    {
        private Action<T1, T2, T3, T4> code;

        public CodeType(string id, string description, string format, Action<T1, T2, T3, T4> code) : base(id, description, format)
        {
            this.code = code;
        }

        public void Invoke(T1 value, T2 value1, T3 value2, T4 value3)
        {
            code.Invoke(value, value1, value2, value3);
        }
    }
}
