using System.Collections.Generic;
using System.Text.Json.Serialization;
using NetDaemon.HassModel.Entities.Core;
using NetDaemon.HassModel.Internal;

namespace NetDaemon.HassModel.Tests.Entities;

public record LightAttributes([property:JsonPropertyName("brightness")]double? Brightness) : ILightService, IOnOffState;