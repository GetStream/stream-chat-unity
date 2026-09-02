using System.Diagnostics;

namespace StreamChat.Core.LowLevelClient
{
    /// <summary>
    /// Elapsed-time source for the per-frame event drain budget. Production uses
    /// <see cref="DiagnosticsElapsedStopwatch"/>; tests inject a fake so pacing is deterministic.
    /// </summary>
    internal interface IElapsedStopwatch
    {
        void Restart();

        double ElapsedMilliseconds { get; }
    }

    internal sealed class DiagnosticsElapsedStopwatch : IElapsedStopwatch
    {
        public void Restart() => _stopwatch.Restart();

        public double ElapsedMilliseconds => _stopwatch.Elapsed.TotalMilliseconds;

        private readonly Stopwatch _stopwatch = new Stopwatch();
    }
}
