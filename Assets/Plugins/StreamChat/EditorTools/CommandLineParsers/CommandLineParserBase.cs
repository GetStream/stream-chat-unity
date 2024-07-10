using System;
using System.Collections.Generic;

namespace StreamChat.EditorTools.CommandLineParsers
{
    public abstract class CommandLineParserBase<TResult>
    {
        public TResult Parse()
        {
            var args = GetParsedCommandLineArguments();
            return Parse(args);
        }

        public IDictionary<string, string> GetParsedCommandLineArguments()
        {
            var result = new Dictionary<string, string>();
            ParseCommandLineArguments(Environment.GetCommandLineArgs(), result);

            return result;
        }

        protected abstract TResult Parse(IDictionary<string, string> args);

        protected void ParseCommandLineArguments(string[] args, IDictionary<string, string> result)
            => ParseCommandLineArguments(args, _ => result.Add(_.Key, _.Value));

        protected void ParseCommandLineArguments(string[] args, Action<(string Key, string Value)> onArgumentParsed)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-"))
                {
                    var key = args[i];
                    var value = i < args.Length - 1 ? args[i + 1] : "";

                    onArgumentParsed?.Invoke((key, value));
                }
            }
        }

        protected static bool IsAnyRequiredArgMissing(IDictionary<string, string> args, out string missingArgsInfo,
            params string[] argKeys)
        {
            var missingKeys = new List<string>();

            foreach (var key in argKeys)
            {
                if (!args.ContainsKey(key))
                {
                    missingKeys.Add(key);
                }
            }

            missingArgsInfo = missingKeys.Count == 0 ? string.Empty : string.Join(", ", missingKeys);
            return missingKeys.Count != 0;
        }
    }
}