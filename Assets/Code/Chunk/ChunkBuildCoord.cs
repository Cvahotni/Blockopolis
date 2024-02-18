using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ChunkBuildCoord
{
    public long coord;
    public bool fromDisk;

    public ChunkBuildCoord(long coord, bool fromDisk) {
        this.coord = coord;
        this.fromDisk = fromDisk;
    }
}
