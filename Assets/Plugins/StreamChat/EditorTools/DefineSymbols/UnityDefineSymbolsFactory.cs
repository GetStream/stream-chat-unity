namespace StreamChat.EditorTools.DefineSymbols
{
    public class UnityDefineSymbolsFactory
    {
        public IUnityDefineSymbols CreateDefault()
        {
#if UNITY_2021_1_OR_NEWER
            return new Unity2021DefineSymbols();
#else
            return new Unity2020DefineSymbols();
#endif
        }
    }
}