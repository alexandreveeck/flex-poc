using System.Runtime.Serialization;

public enum UserUpdateType
{
    [EnumMember(Value = "override")]
    Override,

    [EnumMember(Value = "calendar")]
    Calendar,

    [EnumMember(Value = "forced")]
    Forced
}