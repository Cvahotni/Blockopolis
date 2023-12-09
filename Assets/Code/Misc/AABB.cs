using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABB
{
    public static bool IsOverlapping(float oMinX, float oMinY, float oMinZ, float oMaxX, float oMaxY, float oMaxZ, float minX, float minY, float minZ, float maxX, float maxY, float maxZ) {
        bool xOverlap = oMaxX >= minX && oMinX <= maxX;
        bool yOverlap = oMaxY >= minY && oMinY <= maxY;
        bool zOverlap = oMaxZ >= minZ && oMinZ <= maxZ;

        return xOverlap && yOverlap && zOverlap;
    }
}
