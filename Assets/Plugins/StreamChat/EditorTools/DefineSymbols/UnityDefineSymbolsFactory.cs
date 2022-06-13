namespace StreamChat.EditorTools.DefineSymbols
{
    public class UnityDefineSymbolsFactory
    {
        public IUnityDefineSymbols CreateDefault()
        {
#if UNITY_2021
            return new Unity2021DefineSymbols();
#else
            return new Unity2020DefineSymbols();
#endif
        }
    }
}