using System;
using System.Collections.Generic;
using System.Linq;
using StreamChat.Libs.Auth;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StreamChat.EditorTools
{
    public struct TestAuthDataSet
    {
        public AuthCredentials[] TestAdminData { get; set; }
        public AuthCredentials TestUserData { get; set; }
        public AuthCredentials TestGuestData { get; set; }

        public TestAuthDataSet(IEnumerable<AuthCredentials> testAdminData, AuthCredentials testUserData,
            AuthCredentials testGuestData)
        {
            TestAdminData = testAdminData.ToArray();
            TestUserData = testUserData;
            TestGuestData = testGuestData;
        }

        public AuthCredentials GetAdminData(string forcedAdminId = default, int? forceIndex = default)
        {
            if (!string.IsNullOrEmpty(forcedAdminId))
            {
                return TestAdminData.First(credentials => credentials.UserId == forcedAdminId);
            }

            if (forceIndex.HasValue)
            {
                if (forceIndex < TestAdminData.Length)
                {
                    return TestAdminData[forceIndex.Value];
                }

                Debug.LogWarning(
                    $"{nameof(forceIndex)} is out of range -> given: {forceIndex}, max allowed: {TestAdminData.Length - 1}. Using random admin data instead.");
            }

            return TestAdminData[Random.Range(0, TestAdminData.Length)];
        }

        public AuthCredentials GetOtherThan(AuthCredentials authCredentials)
        {
            for (int i = 0; i < TestAdminData.Length; i++)
            {
                if (TestAdminData[i].UserId == authCredentials.UserId)
                {
                    continue;
                }

                return TestAdminData[i];
            }

            throw new InvalidOperationException(
                $"Failed to find {nameof(AuthCredentials)} other than for user with id: " + authCredentials.UserId);
        }
    }
}