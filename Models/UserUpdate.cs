using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PocFlex.Models;

public class UserUpdate
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("type")]
    [JsonRequired]
    [JsonConverter(typeof(StringEnumConverter))]
    public UserUpdateType Type { get; set; }

    [JsonProperty("column")]
    [JsonRequired]
    [JsonConverter(typeof(StringEnumConverter))]
    public ColumnName Column { get; set; }

    [JsonProperty("force_pbx_profile_id")]
    public string ForcePbxProfileId { get; set; }

    [JsonProperty("schedule_pbx_profile_id")]
    public string SchedulePbxProfileId { get; set; }

    [JsonProperty("profile_pbx_calendar_schedule_id")]
    public string ProfilePbxCalendarScheduleId { get; set; }

    [JsonProperty("default_pbx_profile_id")]
    public string DefaultPbxProfileId { get; set; }
}
