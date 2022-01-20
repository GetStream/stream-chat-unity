using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace StreamChat.Libs.Coroutines
{
    /// <summary>
    /// Runs coroutines
    /// </summary>
    public interface ICoroutineRunner : IDisposable
    {
        Coroutine Run(IEnumerator coroutine);

        void Stop(Coroutine coroutine);

        Task RunAsync(IEnumerator coroutine);
    }
}