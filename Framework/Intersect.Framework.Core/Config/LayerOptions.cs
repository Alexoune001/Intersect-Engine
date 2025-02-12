﻿using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Intersect.Config;

public partial class LayerOptions
{
    public const string Attributes = nameof(Attributes);
    public const string Npcs = nameof(Npcs);
    public const string Lights = nameof(Lights);
    public const string Events = nameof(Events);

    [JsonProperty]
    public List<string> LowerLayers { get; private set; } = ["Ground", "Mask 1", "Mask 2"];

    [JsonProperty]
    public List<string> MiddleLayers { get; private set; } = ["Fringe 1"];

    [JsonProperty]
    public List<string> UpperLayers { get; private set; } = ["Fringe 2"];

    [JsonIgnore]
    public List<string> All { get; private set; } = [];

    [JsonProperty]
    public bool DestroyOrphanedLayers { get; private set; }

    [OnDeserializing]
    internal void OnDeserializingMethod(StreamingContext context)
    {
        All.Clear();
        LowerLayers.Clear();
        MiddleLayers.Clear();
        UpperLayers.Clear();
    }

    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        Validate();
    }

    public void Validate()
    {
        LowerLayers = [..LowerLayers.Distinct()];
        MiddleLayers = [..MiddleLayers.Distinct()];
        UpperLayers = [..UpperLayers.Distinct()];

        var reservedLayers = new string[] { Attributes, Npcs, Lights, Events };
        All.Clear();
        All.AddRange(LowerLayers);
        All.AddRange(MiddleLayers);
        All.AddRange(UpperLayers);

        if (All.Count() == 0)
        {
            //Must have at least 1 map layer!
            throw new Exception("Config Error: You must have at least 1 map layer configured! Please update your server config.");
        }

        foreach (var reserved in reservedLayers)
        {
            if (All.Contains(reserved))
            {
                throw new Exception($"Config Error: Layer '{reserved}' is reserved for editor use. Please choose different naming for map layers in your server config.");
            }
        }

        if (All.Count != All.Distinct().Count())
        {
            //Duplicate layers!
            throw new Exception("Config Error: Duplicate map layers detected! Map layers must be unique in name. Please update your server config.");
        }
    }
}
