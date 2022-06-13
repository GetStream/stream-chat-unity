using StreamChat.Libs.Auth;

namespace StreamChat.Tests
{
    public struct TestAuthDataSet
    {
        public AuthCredentials TestAdminData { get; set; }
        public AuthCredentials TestUserData { get; set; }
        public AuthCredentials TestGuestData { get; set; }
    }
}