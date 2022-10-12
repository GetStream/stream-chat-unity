using System.Threading.Tasks;
using StreamChat.Core.State;
using StreamChat.Libs.Auth;
using StreamChat.Libs.Logs;
using StreamChat.Libs.Utils;
using UnityEngine;

public class TestChatBehaviour : MonoBehaviour
{
    protected async void Awake()
    {
        _client = StreamChatStateClient.CreateDefaultClient();

        ConnectToStream().LogIfFailed(_unityLogger);
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

    private readonly ILogs _unityLogger = new UnityLogs();

    [SerializeField]
    private AuthCredentialsAsset _authCredentialsAsset;

    private IStreamChatStateClient _client;

    private async Task ConnectToStream()
    {
        var localUser = await _client.ConnectUserAsync(_authCredentialsAsset.Credentials);

        Debug.Log("Logged in with user id: " + localUser.Id);

        await FetchChannels();
    }

    private async Task FetchChannels()
    {
        var channel = await _client.GetOrCreateChannelAsync(ChannelType.Messaging, "my-channel-id");

        var isNull = channel == null;

        Debug.Log("Returned channel: " + (isNull ? "None" : channel.Cid));

        if (!isNull)
        {
            foreach (var msg in channel.Messages)
            {
                Debug.Log("Received message: " + msg);
            }
        }
    }
}
