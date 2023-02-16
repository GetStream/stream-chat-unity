#if STREAM_TESTS_ENABLED
using System;
using NUnit.Framework.Interfaces;
using StreamChat.Tests;
using UnityEngine.TestRunner;

[assembly: TestRunCallback(typeof(UnityTestRunnerCallbacks))]

namespace StreamChat.Tests
{
    /// <summary>
    /// Receives callback from Unity Editor
    /// </summary>
    internal class UnityTestRunnerCallbacks : ITestRunCallback
    {
        public static UnityTestRunnerCallbacks Instance { get; private set;}

        public static event Action<ITest> RunStartedCallback;
        public static event Action<ITestResult> RunFinishedCallback;
        public static event Action<ITest> TestStartedCallback;
        public static event Action<ITestResult> TestFinishedCallback;
        
        public UnityTestRunnerCallbacks()
        {
            Instance = this;
        }

        public void RunStarted(ITest testsToRun) => RunStartedCallback?.Invoke(testsToRun);

        public void RunFinished(ITestResult testResults) => RunFinishedCallback?.Invoke(testResults);

        public void TestStarted(ITest test) => TestStartedCallback?.Invoke(test);

        public void TestFinished(ITestResult result) => TestFinishedCallback?.Invoke(result);
    }
}
#endif