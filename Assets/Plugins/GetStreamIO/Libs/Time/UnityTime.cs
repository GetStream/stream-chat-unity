namespace StreamChat.Libs.Time
{
    /// <summary>
    /// <see cref="ITimeService"/> based on <see cref="UnityEngine.Time"/>
    /// </summary>
    public class UnityTime : ITimeService
    {
        public float Time => UnityEngine.Time.time;
        public float DeltaTime => UnityEngine.Time.deltaTime;
    }
}