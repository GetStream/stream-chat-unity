﻿using StreamChat.Core.Requests;

namespace StreamChat.Core.Requests
{
    public class ShadowBanRequest : BanRequest
    {
        public new bool? Shadow => true;
    }
}