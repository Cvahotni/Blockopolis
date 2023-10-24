using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkBorder {
    public enum ChunkBorderDirection {
        UP, 
        DOWN, 
        LEFT, 
        RIGHT,
        UP_LEFT, 
        UP_RIGHT,
        DOWN_LEFT, 
        DOWN_RIGHT, 
        UNDEFINED
    }

    public static readonly Vector2Int[] Axes = new Vector2Int[] {
        new Vector2Int(0, -1),
        new Vector2Int(0, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(-1, -1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(1, 1),
        new Vector2Int(0, 0)
    };
}