using System.Runtime.Serialization;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public enum DocumentMetadataDataType
    {
        [EnumMember(Value = "string")]
        String = 0,

        [EnumMember(Value = "boolean")]
        Boolean = 1,

        [EnumMember(Value = "date-time")]
        DateTime = 2,

        [EnumMember(Value = "date")]
        Date = 3,

        [EnumMember(Value = "integer")]
        Integer = 4,

        [EnumMember(Value = "number")]
        Number = 5
    }
}
