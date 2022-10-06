using StreamChat.Core.State;
using StreamChat.Libs.Auth;
using UnityEngine;

public class TestChatBehaviour : MonoBehaviour
{
    protected void Awake()
    {
        _client = StreamChatStateClient.CreateDefaultClient();
        _client.ConnectUserAsync(_authCredentialsAsset.Credentials).ContinueWith(_ =>
        {
            if (_.IsFaulted)
            {
                Debug.LogException(_.Exception);
                return;
            }

            Debug.Log("Logged in with user id: " + _.Result.Id);
        });
    }

    protected void Update()
    {
        _client.Update();
    }

    protected void OnDestroy()
    {
        _client.Dispose();
        _client = null;
    }

    [SerializeField]
    private AuthCredentialsAsset _authCredentialsAsset;

    private IStreamChatStateClient _client;
}
