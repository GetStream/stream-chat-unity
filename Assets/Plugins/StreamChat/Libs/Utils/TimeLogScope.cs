using System;
using System.Text;

namespace StreamChat.Libs.Utils
{
    /// <summary>
    /// Measures real time elapsed during scope and prints to logger
    /// </summary>
    public readonly struct TimeLogScope : IDisposable
    {
        public TimeLogScope(string name, Action<string> logger)
        {
            if (name.IsNullOrEmpty())
            {
                throw new ArgumentException(nameof(name));
            }

            _name = name;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sb = new StringBuilder();

            _timeStarted = UnityEngine.Time.realtimeSinceStartup;
        }

        public void Dispose()
        {
            var elapsed = UnityEngine.Time.realtimeSinceStartup - _timeStarted;

            _sb.Append(_name);
            _sb.Append(" - Executed in: ");
            _sb.Append(elapsed);

            _logger(_sb.ToString());
        }

        private readonly string _name;
        private readonly Action<string> _logger;
        private readonly StringBuilder _sb;

        private readonly float _timeStarted;
    }
}