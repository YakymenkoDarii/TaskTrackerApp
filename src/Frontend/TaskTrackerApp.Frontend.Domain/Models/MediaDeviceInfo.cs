using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Frontend.Domain.Models;

public class MediaDeviceInfo
{
    [System.Text.Json.Serialization.JsonPropertyName("deviceId")]
    public string DeviceId { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("kind")]
    public string Kind { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("label")]
    public string Label { get; set; }
}