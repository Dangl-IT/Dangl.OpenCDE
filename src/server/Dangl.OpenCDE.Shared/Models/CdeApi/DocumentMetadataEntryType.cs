using System.Runtime.Serialization;

namespace Dangl.OpenCDE.Shared.Models.CdeApi
{
    public enum DocumentMetadataEntryType
    {
        [EnumMember(Value = "string")]
        String = 0,

        [EnumMember(Value = "boolean")]
        Boolean = 1,

        [EnumMember(Value = "date-time")]
        DateTime = 2,

        [EnumMember(Value = "integer")]
        Integer = 3,

        [EnumMember(Value = "number")]
        Number = 4,

        [EnumMember(Value = "enum")]
        Enum = 5,

        [EnumMember(Value = "array")]
        Array = 6
    }
}
