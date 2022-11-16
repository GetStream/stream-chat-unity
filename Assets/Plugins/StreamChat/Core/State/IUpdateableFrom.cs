﻿using StreamChat.Core.State.Caches;
using StreamChat.Core;

namespace StreamChat.Core.State
{
    internal interface IUpdateableFrom<in TDto, out TTrackedObject>
        where TTrackedObject : IStreamTrackedObject, IUpdateableFrom<TDto, TTrackedObject>
    {
        void UpdateFromDto(TDto dto, ICache cache);
    }
}