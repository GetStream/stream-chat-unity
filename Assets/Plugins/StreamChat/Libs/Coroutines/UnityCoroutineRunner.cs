using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace StreamChat.Libs.Coroutines
{
    /// <summary>
    /// Unity implementation for <see cref="ICoroutineRunner"/>
    /// </summary>
    public class UnityCoroutineRunner : MonoBehaviour, ICoroutineRunner
    {
        public static ICoroutineRunner Create()
        {
            var runnerGameObject = new GameObject();
            return runnerGameObject.AddComponent<UnityCoroutineRunner>();
        }

        public Coroutine Run(IEnumerator coroutine)
            => StartCoroutine(coroutine);

        public void Stop(Coroutine coroutine)
            => StopCoroutine(coroutine);

        public Task RunAsync(IEnumerator coroutine)
        {
            var tcs = new TaskCompletionSource<bool>();

            StartCoroutine(WaitForCoroutine(coroutine, tcs));

            return tcs.Task;
        }

        public void Dispose()
            => Destroy(this);

        protected void Awake()
        {
            DontDestroyOnLoad(this);
            name = "StreamChatClient - Coroutine Runner";
        }

        private readonly GameObject _coroutineRunnerGameObject;

        private static IEnumerator WaitForCoroutine(IEnumerator innerCoroutine, TaskCompletionSource<bool> tcs)
        {
            yield return innerCoroutine;
            tcs.SetResult(true);
        }

        /// <summary>
        /// MonoBehaviour should not be instantiated with a constructor. Please use <see cref="Create"/> method
        /// </summary>
        private UnityCoroutineRunner()
        {
        }
    }
}