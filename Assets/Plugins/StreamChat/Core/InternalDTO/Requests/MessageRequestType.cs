﻿//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.15.5.0 (NJsonSchema v10.6.6.0 (Newtonsoft.Json v9.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------


using StreamChat.Core.InternalDTO.Responses;
using StreamChat.Core.InternalDTO.Events;
using StreamChat.Core.InternalDTO.Models;

namespace StreamChat.Core.InternalDTO.Requests
{
    using System = global::System;

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.15.5.0 (NJsonSchema v10.6.6.0 (Newtonsoft.Json v9.0.0.0))")]
    public enum MessageRequestType
    {

        [System.Runtime.Serialization.EnumMember(Value = @"regular")]
        Regular = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"ephemeral")]
        Ephemeral = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"error")]
        Error = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"reply")]
        Reply = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"system")]
        System = 4,

        [System.Runtime.Serialization.EnumMember(Value = @"deleted")]
        Deleted = 5,

    }

}

