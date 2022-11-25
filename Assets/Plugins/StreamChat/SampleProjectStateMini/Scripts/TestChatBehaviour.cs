using System.Threading.Tasks;
using StreamChat.Core;
using StreamChat.Libs.Auth;
using StreamChat.Libs.Utils;
using UnityEngine;

public class TestChatBehaviour : MonoBehaviour
{
    protected void Awake()
    {
        _client = StreamChatClient.CreateDefaultClient();

        ConnectToStream().LogIfFailed();
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
