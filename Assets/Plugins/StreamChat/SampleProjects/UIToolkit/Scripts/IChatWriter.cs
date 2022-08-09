using System.Threading.Tasks;

namespace StreamChat.SampleProjects.UIToolkit
{
    public interface IChatWriter
    {
        Task SendNewMessageAsync(string message);
    }
}