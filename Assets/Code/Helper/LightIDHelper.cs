public class LightIDHelper
{
    public static byte Level(byte input) {
        return (byte) (input & 0x0F);
    }

    public static byte Pack(byte level, byte color) {
        return (byte)((color << 4) | level);
    }
}
