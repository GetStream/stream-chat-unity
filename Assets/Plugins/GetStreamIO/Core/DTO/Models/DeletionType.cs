//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.15.5.0 (NJsonSchema v10.6.6.0 (Newtonsoft.Json v9.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------


using GetStreamIO.Core.DTO.Responses;
using GetStreamIO.Core.DTO.Requests;
using GetStreamIO.Core.DTO.Events;

namespace GetStreamIO.Core.DTO.Models
{
    using System = global::System;

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.15.5.0 (NJsonSchema v10.6.6.0 (Newtonsoft.Json v9.0.0.0))")]
    public enum DeletionType
    {

        [System.Runtime.Serialization.EnumMember(Value = @"soft")]
        Soft = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"pruning")]
        Pruning = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"hard")]
        Hard = 2,

    }

}

