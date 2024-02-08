using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BlockID
{
    public byte id;
    public byte variant;
    public byte rotation;

    public BlockID(byte id, byte variant, byte rotation) {
        this.id = id;
        this.variant = variant;
        this.rotation = rotation;
    }

    public BlockID(byte id, byte variant) {
        this.id = id;
        this.variant = variant;
        this.rotation = 0;
    }

    public BlockID(byte id) {
        this.id = id;
        this.variant = 0;
        this.rotation = 0;
    }

    public static BlockID FromUShort(ushort id) {
        byte currentID = (byte) id;
        byte currentVariant = 0;

        return new BlockID(currentID, currentVariant);
    }

    public BlockID(ushort packedID) {
        this.id = BlockIDHelper.ID(packedID);
        this.variant = BlockIDHelper.Variant(packedID);
        this.rotation = BlockIDHelper.Rotation(packedID);
    }

    public bool IsAir() {
        return id == 0;
    }

    public ushort Pack() {
        return BlockIDHelper.Pack(id, variant, rotation);
    }

    public bool Equals(BlockID other) {
        return other.id == id && other.variant == variant && other.rotation == rotation;
    }
}
