namespace StreamChat.SampleProject.Pooling
{
    public interface IPoolItem
    {
        void OnRenting();

        void OnReturning();
    }
}