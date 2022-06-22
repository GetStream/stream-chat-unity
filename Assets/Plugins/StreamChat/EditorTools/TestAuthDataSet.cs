using StreamChat.Libs.Auth;

namespace StreamChat.EditorTools
{
    public struct TestAuthDataSet
    {
        public AuthCredentials TestAdminData { get; set; }
        public AuthCredentials TestUserData { get; set; }
        public AuthCredentials TestGuestData { get; set; }
    }
}