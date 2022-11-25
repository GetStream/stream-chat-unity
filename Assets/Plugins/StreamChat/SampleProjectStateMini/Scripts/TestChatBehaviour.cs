using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Core.Helpers;
using StreamChat.Libs.Auth;
using UnityEngine;

public class TestChatBehaviour : MonoBehaviour
{
    protected void Awake()
    {
        _client = StreamChatClient.CreateDefaultClient();

        ConnectToStream().LogExceptionsOnFailed();
    }

    [SerializeField]
    private AuthCredentialsAsset _authCredentialsAsset;

    private IStreamChatClient _client;

    private async Task ConnectToStream()
    {
        var localUserData = await _client.ConnectUserAsync(_authCredentialsAsset.Credentials);

        Debug.Log("Logged in with user id: " + localUserData.User);

        await FetchChannels();
    }

    private async Task FetchChannels()
    {
        var channel = await _client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

        foreach (var msg in channel.Messages)
        {
            Debug.Log("Received message: " + msg);
        }
    }
}
