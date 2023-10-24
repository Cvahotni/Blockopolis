using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelModification
{
    private Vector3Int position;
    private ushort id;

    public Vector3Int Position { get { return position; }}
    public ushort ID { get { return id; }}

    public VoxelModification(Vector3Int position, ushort id) {
        this.position = position;
        this.id = id;
    }
}
