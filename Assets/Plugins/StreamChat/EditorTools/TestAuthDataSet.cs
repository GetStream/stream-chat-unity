using System.Collections.Generic;
using System.Linq;
using StreamChat.Libs.Auth;
using Random = UnityEngine.Random;

namespace StreamChat.EditorTools
{
    public struct TestAuthDataSet
    {
        public AuthCredentials[] TestAdminData { get; set; }
        public AuthCredentials TestUserData { get; set; }
        public AuthCredentials TestGuestData { get; set; }

        public TestAuthDataSet(IEnumerable<AuthCredentials> testAdminData, AuthCredentials testUserData, AuthCredentials testGuestData)
        {
            TestAdminData = testAdminData.ToArray();
            TestUserData = testUserData;
            TestGuestData = testGuestData;
        }

        public AuthCredentials GetRandomAdminData() => TestAdminData[Random.Range(0, TestAdminData.Length)];
    }
}