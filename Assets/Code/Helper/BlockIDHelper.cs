public class BlockIDHelper
{
    public static byte ID(ushort input) {
        return (byte)(input >> 8);
    }

    public static byte Variant(ushort input) {
        return (byte) (LowerByte(input) & 0xFC);
    }

    public static byte Rotation(ushort input) {
        return (byte) (LowerByte(input) & 0x03);
    }

    public static byte LowerByte(ushort input) { 
        return (byte) (input & 255);
    }

    public static ushort Pack(byte id, byte variant, byte rotation) {
        return (ushort) ((id << 8) + ((variant << 2) | rotation));
    }

    public static ushort Pack(byte id, byte variant) {
        return Pack(id, variant, 0);
    }

    public static ushort PackEmpty() {
        return Pack(0, 0, 0);
    }
}
