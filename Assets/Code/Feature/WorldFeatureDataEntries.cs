using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class WorldFeatureDataEntries
{
    [SerializeField] private ushort id;
    [SerializeField] private WorldFeatureDataEntry[] data;

    public ushort ID { get { return id; }}

    public WorldFeatureDataEntries(ushort id, List<WorldFeatureDataEntry> entries) {
        this.id = id;
        data = entries.ToArray();
    }

    public Dictionary<FeaturePlacement, ushort> ToDictionary() {
        Dictionary<FeaturePlacement, ushort> dictionary = new Dictionary<FeaturePlacement, ushort>();

        for(int i = 0; i < data.Length; i++) {
            WorldFeatureDataEntry entry = data[i];
            dictionary.Add(new FeaturePlacement(entry.Pos.x, entry.Pos.y, entry.Pos.z, id), entry.Block.Pack());
        }

        return dictionary;
    }
}
