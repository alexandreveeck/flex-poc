using System.Runtime.Serialization;

namespace PocFlex.Models;

public enum ColumnName
{
    [EnumMember(Value = "schedule_pbx_profile_id")]
    Schedule,

    [EnumMember(Value = "force_pbx_profile_id")]
    Force,

    [EnumMember(Value = "default_pbx_profile_id")]
    Default
}