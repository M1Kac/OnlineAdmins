using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace OnlineAdmins
{
    public class Config : BasePluginConfig
    {
        [JsonPropertyName("AdminFlag")]
        public string AdminFlag { get; set; } = "css/generic";

        [JsonPropertyName("Time")]
        public float Time { get; set; } = 10.0f;

        [JsonPropertyName("HudColor")]
        public string HudColor { get; set; } = "White";
    }
}
