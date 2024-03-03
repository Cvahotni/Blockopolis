using System;

public struct FeaturePlacement : IEquatable<FeaturePlacement>
{
    public int x;
    public int y;
    public int z;

    public ushort id;

    public FeaturePlacement(int x, int y, int z, ushort id) {
        this.x = x;
        this.y = y;
        this.z = z;

        this.id = id;
    }

    public bool Equals(FeaturePlacement other) {
        return other.x == x && other.y == y && other.z == z && other.id == id;
    }

    public override int GetHashCode() {
        return id.GetHashCode();
    }
}
