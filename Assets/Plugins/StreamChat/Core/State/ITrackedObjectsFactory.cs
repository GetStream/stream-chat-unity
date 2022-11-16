﻿using StreamChat.Core.StatefulModels;

namespace StreamChat.Core.State
{
    internal interface ITrackedObjectsFactory
    {
        StreamChannel CreateStreamChannel(string uniqueId);

        StreamChannelMember CreateStreamChannelMember(string uniqueId);

        StreamLocalUserData CreateStreamLocalUser(string uniqueId);

        StreamMessage CreateStreamMessage(string uniqueId);

        StreamUser CreateStreamUser(string uniqueId);
    }
}